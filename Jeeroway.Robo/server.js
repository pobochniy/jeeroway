'use strict';
const isDev = 1;
const overmindHost = isDev ? 'localhost' : '192.168.0.105';
const overmindPort = isDev ? '54108' : '5000';
const bodyParser = require('body-parser');
const express = require('express');
const led = require('./controllers/led');
const stream = require('./controllers/stream');


const app = express();
const port = 3000;


// app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

app.use('/led', led);
app.use('/stream', stream)


app.use(function (req, res, next) {
    const err = new Error('Not Found');
    err.status = 404;
    next(err);
});


app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

console.log(process.env.PORT);
// app.set('port', process.env.PORT || 5000);

app.listen(port, ()=>{
    console.log(`Raspberry pi streaming control on port ${port}`);
});