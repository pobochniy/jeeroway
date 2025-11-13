'use strict';
const express = require('express');
const {spawn} = require("child_process");
const router = express.Router();

let ffmpegProcess = null;
router.post('/start', (req, res) => {
    if (ffmpegProcess) {
        return res.status(400).send('streaming already started');
    }

    const udpAddress = '192.168.1.33:5000';
    const args = [
        '-f', 'v4l2',
        '-framerate', '15',
        '-input_format', 'yuyv422',
        '-video_size', '640x480',
        '-i', '/dev/video0',
        '-vcodec', 'mjpeg',
        '-f', 'mjpeg',
        'udp://' + udpAddress
    ];

    console.log('Launch ffmpeg with args:', args.join(' '));

    ffmpegProcess = spawn('ffmpeg', args);

    ffmpegProcess.stderr.on('data', data => {
        console.error(`ffmpeg: ${data}`);
    });

    ffmpegProcess.on('exit', (code) => {
        console.log(`ffmpeg exited with code ${code}`);
        ffmpegProcess = null;
    });
    res.send('streaming started');
});

router.post('/stop', (req, res) => {
    if (!ffmpegProcess) {
        return res.status(400).send('No active stream');
    }

    ffmpegProcess.kill('SIGINT');
    ffmpegProcess = null;
    res.send('Streaming stopped');
});

module.exports = router;
