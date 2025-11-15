К RPI подключается Iskra.js
Это плата, аналог ардуино, но работает на js коде

есть два способа подключения:
первый, вручную через USB
этим способом можно залить первый скрипт (из папки boot.js)
или повыполнять какой-то код вручную
  ttyACM0 — USB CDC (Virtual COM Port)
  для управления этой платой используется espruino web ide
  должна работать прямо через сайт https://www.espruino.com/ide
  если с этим проблемы, можем попробовать установить
  ```curl -sL https://deb.nodesource.com/setup_7.x | sudo -E bash -```
  ```sudo npm install -g espruino-web-ide```
  ```sudo setcap cap_net_raw+eip $(eval readlink -f `which node`)```

второй, из node.js кода командами
  /dev/ttyAMA0 — основной аппаратный UART Raspberry Pi
  для этого необходимо отключить блютуз на rpi
  ```sudo nano /boot/config.txt```
  ```dtoverlay=disable-bt```
  ```enable_uart=1```
  и поставить пины RxTx
  