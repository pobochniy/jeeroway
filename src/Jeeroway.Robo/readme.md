# Как запустить на RPi

1. Установить системные зависимости (для сборки serialport, если не подтянется prebuild):

```bash
sudo apt-get update && sudo apt-get install -y build-essential python3 make g++
```

2. Доступ к последовательному порту без sudo (группа dialout), затем перелогиниться:

```bash
sudo usermod -a -G dialout $USER
```

3. Установить npm-зависимости (в каталоге `Jeeroway.Robo/`):

```bash
npm install
```

4. Запуск сервиса. По умолчанию путь `/dev/ttyACM0`, скорость `115200`.
   Для явной настройки используйте переменные окружения `SERIAL_PATH` и `SERIAL_BAUD`:

```bash
# По умолчанию
npm start

# С явной настройкой
SERIAL_PATH=/dev/serial/by-id/usb-Arduino... SERIAL_BAUD=115200 npm start
```

5. Проверка эндпоинтов:

```bash
# Статус последовательного порта
curl http://localhost:3000/led/status

# Отправка стандартной команды "go();"
curl http://localhost:3000/led

# Отправка произвольной команды
curl -X POST http://localhost:3000/led/send \
     -H 'Content-Type: application/json' \
     -d '{"data":"go();"}'
```

## Видеопоток (FFmpeg)

Если используется стриминг камеры через FFmpeg:

```bash
# Запуск стрима
curl -X POST http://127.0.0.1:3000/stream/start

# Остановка стрима
curl -X POST http://127.0.0.1:3000/stream/stop

# Полезно: доступ к видео-устройству (группа video) и диагностика устройств
sudo usermod -aG video $USER
ls -l /dev/video* && v4l2-ctl --list-devices
```

# Полезные проверки и советы

- **Найти реальный путь порта.** Удобно использовать by-id:

```bash
ls -l /dev/serial/by-id/
dmesg | grep -i tty
```

- **UART-консоль.** Для аппаратного UART убедитесь, что консоль не заняла порт (актуально без USB‑Serial).

- **Строковые окончания.** Если скетч Arduino ждёт `\n` или `\r\n`, добавляйте соответствующий суффикс к команде (`services/serial.js` и `POST /led/send`).

- **Права доступа.** Если нет доступа к `/dev/ttyACM0`, проверьте членство в `dialout` и перелогиньтесь.

- **Параметры через env.** `SERIAL_PATH`, `SERIAL_BAUD` можно передавать при запуске.
