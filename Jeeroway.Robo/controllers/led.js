'use strict';
const express = require('express');
const router = express.Router();

const serial = require('../services/serial');

// GET /led -> send a default command and return serial status
router.get('/', async (req, res) => {
    const cmd = 'go();';
    try {
        await serial.send(cmd);
        res.json({ ok: true, sent: cmd.trim(), status: serial.getStatus() });
    } catch (e) {
        res.status(500).json({ ok: false, error: String(e), status: serial.getStatus() });
    }
});

// POST /led/send { data: "..." } or ?data=... / ?cmd=...
router.post('/send', async (req, res) => {
    const bodyData = typeof req.body?.data === 'string' ? req.body.data : req.body?.cmd;
    const queryData = typeof req.query?.data === 'string' ? req.query.data : req.query?.cmd;
    const data = bodyData || queryData;
    if (!data) return res.status(400).json({ ok: false, error: 'data is required' });

    const payload = data.endsWith('\n') ? data : data + '\n';
    try {
        await serial.send(payload);
        res.json({ ok: true, sent: payload.trim() });
    } catch (e) {
        res.status(500).json({ ok: false, error: String(e) });
    }
});

// GET /led/status -> current serial port status
router.get('/status', (req, res) => {
    res.json(serial.getStatus());
});

module.exports = router;
