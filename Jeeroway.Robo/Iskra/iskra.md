К RPI подключается Iskra.js
Это плата, аналог ардуино, но работает на js коде

есть два способа подключения:
первый, вручную через USB
этим способом можно залить первый скрипт (из папки boot.js)
или повыполнять какой-то код вручную
  ttyACM0 — USB CDC (Virtual COM Port)
  для управления этой платой используется espruino web ide
https://github.com/espruino/EspruinoWebIDE
  должна работать прямо через сайт https://www.espruino.com/ide
добавим кастомные модули амперки
```https://www.espruino.com/modules | https://js.amperka.ru/modules```

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

далее запустим node server.js
можем вызвать /led/status
Если выдает
{"path":"/dev/ttyAMA0","baudRate":9600,"isOpen":false,"opening":false,"queueLength":1,"stats":{"openCount":0,"bytesWritten":0,"messagesWritten":0,"lastOpenAt":null,"lastCloseAt":null,"lastError":"Error: Permission denied, cannot open /dev/ttyAMA0"}}

```bash
ls -l /dev/ttyAMA0
```
надо добавить пользователя в группу
```bash
sudo usermod -aG dialout firstvds
# или, если устройство в группе tty:
sudo usermod -aG tty firstvds
```
так же разрешить 3000 порт (стандартный для node.js), на котором запускается сервер, чтобы апи rpi было доступно извне
```bash
sudo ufw allow 3000
```
