'use strict';
const express = require('express');
const router = express.Router();

const raspi = require('raspi').init;
const Serial = require('raspi-serial').Serial;

router.get('/', function (req, res) {
    var kk = req.query.p;
    console.log(kk);

    var serial = new Serial('/dev/ttyacm0/');
    serial.open(() => {
        console.log('koko');
        serial.write('go();');
        console.log('LED1.write('+kk+');');
    });

    res.send(kk ? 'led on' : 'led off');
});

module.exports = router;
