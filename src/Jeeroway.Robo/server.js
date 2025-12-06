'use strict';
const isDev = 1;
const overmindHost = isDev ? 'localhost' : '192.168.0.105';
const overmindPort = isDev ? '54108' : '5000';
const express = require('express');
const led = require('./controllers/led');
const stream = require('./controllers/stream');
const hub = require('./controllers/hub');


const app = express();
const port = 3000;


// app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

app.use('/led', led);
app.use('/stream', stream)
app.use('/hub', hub)


app.use(function (req, res, next) {
    const err = new Error('Not Found');
    err.status = 404;
    next(err);
});


app.use(function (err, req, res, next) {
    const status = err.status || 500;
    res.status(status).json({
        message: err.message || 'Internal Server Error'
    });
});

console.log(process.env.PORT);
// app.set('port', process.env.PORT || 5000);

app.listen(port, ()=>{
    console.log(`Raspberry pi streaming control on port ${port}`);
});