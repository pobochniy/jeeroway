'use strict';

const express = require('express');
const router = express.Router();
const SignalRClient = require('../services/signalr-client');
const serial = require('../services/serial');
const { spawn } = require('child_process');

const isDev = 0;
const overmindHost = isDev ? 'localhost' : '192.168.1.33';
const overmindPort = '5000';
const hubUrl = `http://${overmindHost}:${overmindPort}/hub/robocontrol`;

// TODO: Получать roboId из конфигурации или переменных окружения
const roboId = process.env.ROBO_ID || '00000000-0000-0000-0000-000000000001';

const signalRClient = new SignalRClient(hubUrl, roboId, 10000);

let ffmpegProcess = null;

// Обработчик команд управления от API
signalRClient.on('SendControlToRobo', async (dto) => {
    console.log('[Hub] Received control command:', dto);
    
    try {
        // Формируем команду для Arduino на основе WASD
        const w = dto.w ? 1 : 0;
        const s = dto.s ? 1 : 0;
        const a = dto.a ? 1 : 0;
        const d = dto.d ? 1 : 0;
        
        const cmd = `go(${w},${s},${a},${d});`;
        console.log('[Hub] Sending to serial:', cmd);
        
        await serial.send(cmd);
    } catch (error) {
        console.error('[Hub] Error processing control command:', error.message);
    }
});

// Обработчик команды старта видеострима
signalRClient.on('StartVideoStream', async (config) => {
    console.log('[Hub] Received StartVideoStream command:', config);
    
    try {
        if (ffmpegProcess) {
            console.warn('[Hub] Video stream already running');
            return;
        }

        const udpAddress = config?.udpAddress || '192.168.1.33:5000';
        const framerate = config?.framerate || 15;
        const videoSize = config?.videoSize || '640x480';
        
        const args = [
            '-f', 'v4l2',
            '-framerate', String(framerate),
            '-input_format', 'yuyv422',
            '-video_size', videoSize,
            '-i', '/dev/video0',
            '-vcodec', 'mjpeg',
            '-f', 'mjpeg',
            `udp://${udpAddress}`
        ];

        console.log('[Hub] Starting ffmpeg with args:', args.join(' '));

        ffmpegProcess = spawn('ffmpeg', args);

        ffmpegProcess.stderr.on('data', data => {
            console.error(`[ffmpeg] ${data}`);
        });

        ffmpegProcess.on('exit', (code) => {
            console.log(`[ffmpeg] Exited with code ${code}`);
            ffmpegProcess = null;
        });

        console.log('[Hub] Video stream started successfully');
    } catch (error) {
        console.error('[Hub] Error starting video stream:', error.message);
    }
});

// Обработчик команды остановки видеострима
signalRClient.on('StopVideoStream', async () => {
    console.log('[Hub] Received StopVideoStream command');
    
    try {
        if (!ffmpegProcess) {
            console.warn('[Hub] No active video stream to stop');
            return;
        }

        ffmpegProcess.kill('SIGINT');
        ffmpegProcess = null;
        console.log('[Hub] Video stream stopped successfully');
    } catch (error) {
        console.error('[Hub] Error stopping video stream:', error.message);
    }
});

// Запускаем подключение к SignalR хабу при загрузке модуля
signalRClient.start().catch(err => {
    console.error('[Hub] Failed to start SignalR client:', err.message);
});

// REST endpoints для тестирования и мониторинга
router.get('/status', (req, res) => {
    res.json({
        signalr: {
            state: signalRClient.getState(),
            connected: signalRClient.isConnected(),
            roboId: roboId
        },
        serial: serial.getStatus(),
        videoStream: {
            active: ffmpegProcess !== null
        }
    });
});

router.post('/test-invoke', async (req, res) => {
    try {
        if (!signalRClient.isConnected()) {
            return res.status(503).json({ ok: false, error: 'SignalR not connected' });
        }
        
        const method = req.body?.method || 'PushMessage';
        const data = req.body?.data || { message: 'Test from robot' };
        
        await signalRClient.invoke(method, data);
        res.json({ ok: true, method, data });
    } catch (error) {
        res.status(500).json({ ok: false, error: error.message });
    }
});

router.get('/reconnect', async (req, res) => {
    try {
        console.log('[Hub] Manual reconnect requested');
        await signalRClient.start();
        res.json({ ok: true, state: signalRClient.getState() });
    } catch (error) {
        res.status(500).json({ ok: false, error: error.message });
    }
});

// Graceful shutdown
process.on('SIGINT', async () => {
    console.log('[Hub] Shutting down...');
    
    if (ffmpegProcess) {
        ffmpegProcess.kill('SIGINT');
    }
    
    await signalRClient.stop();
    process.exit(0);
});

module.exports = router;
