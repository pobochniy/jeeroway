# Jeeroway.Robo - Robot Control Service

Сервис управления роботом на Node.js с поддержкой SignalR, serial-порта и видеострима.

## Возможности

- **SignalR клиент** - постоянное соединение с API, автоматическое переподключение каждые 10 секунд
- **Serial порт** - управление Arduino через последовательный порт
- **Видеострим** - трансляция видео через FFmpeg
- **REST API** - локальные эндпоинты для тестирования и мониторинга

## Быстрый старт

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
   Для явной настройки используйте переменные окружения:

```bash
# По умолчанию
npm start

# С настройками
ROBO_ID=12345678-1234-1234-1234-123456789abc \
SERIAL_PATH=/dev/serial/by-id/usb-Arduino... \
SERIAL_BAUD=115200 \
npm start
```

## Переменные окружения

- `ROBO_ID` - уникальный идентификатор робота (GUID), по умолчанию `00000000-0000-0000-0000-000000000001`
- `SERIAL_PATH` - путь к serial порту, по умолчанию `/dev/ttyACM0`
- `SERIAL_BAUD` - скорость порта, по умолчанию `115200`

## Проверка работы

### SignalR и общий статус

```bash
# Статус всех компонентов (SignalR, serial, video)
curl http://localhost:3000/hub/status
```

### Serial порт

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

### Видеопоток (FFmpeg)

```bash
# Запуск стрима (локально через REST)
curl -X POST http://127.0.0.1:3000/stream/start

# Остановка стрима
curl -X POST http://127.0.0.1:3000/stream/stop

# Полезно: доступ к видео-устройству (группа video) и диагностика устройств
sudo usermod -aG video $USER
ls -l /dev/video* && v4l2-ctl --list-devices
```

**Примечание:** Видеострим также управляется через SignalR командами `StartVideoStream` и `StopVideoStream` от API.

## Архитектура

```
Browser (Angular)
    ↓ SignalR
API (RoboHub)
    ↓ SignalR Group (robo_{roboId})
Robot (Node.js)
    ↓
    ├─ Serial Port → Arduino
    └─ FFmpeg → Video Stream
```

Подробнее см. [SIGNALR.md](./SIGNALR.md)

## Полезные проверки и советы

- **Найти реальный путь порта.** Удобно использовать by-id:

```bash
ls -l /dev/serial/by-id/
dmesg | grep -i tty
```

- **UART-консоль.** Для аппаратного UART убедитесь, что консоль не заняла порт (актуально без USB‑Serial).

- **Строковые окончания.** Если скетч Arduino ждёт `\n` или `\r\n`, добавляйте соответствующий суффикс к команде (`services/serial.js` и `POST /led/send`).

- **Права доступа.** Если нет доступа к `/dev/ttyACM0`, проверьте членство в `dialout` и перелогиньтесь.

- **Параметры через env.** `SERIAL_PATH`, `SERIAL_BAUD`, `ROBO_ID` можно передавать при запуске.

- **SignalR переподключение.** Робот автоматически переподключается к API каждые 10 секунд при разрыве соединения.
