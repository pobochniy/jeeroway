node server.js
curl -X POST http://127.0.0.1:3000/start

sudo usermod -aG video firstvds
ls -l /dev/video* && v4l2-ctl --list-devices

