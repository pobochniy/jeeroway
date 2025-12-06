# SignalR Integration для Jeeroway.Robo

## Обзор

Робот подключается к API через SignalR Hub (`/robohub`) и поддерживает постоянное соединение с автоматическим переподключением.

## Архитектура

### Компоненты

1. **SignalRClient** (`services/signalr-client.js`) - основной клиент с автоматическим переподключением
2. **Hub Controller** (`controllers/hub.js`) - обработчики команд от API
3. **RoboHub** (API) - серверная часть SignalR хаба

### Поток данных

```
Browser → API (RoboHub.ReceiveControlFromBrowser) 
    → SignalR Group (robo_{roboId})
    → Robot Client (SendControlToRobo handler)
    → Serial/ffmpeg
```

## Конфигурация

### Переменные окружения

- `ROBO_ID` - уникальный идентификатор робота (GUID)
- По умолчанию: `00000000-0000-0000-0000-000000000001`

### Настройки подключения

В `controllers/hub.js`:

```javascript
const isDev = 1;
const overmindHost = isDev ? 'localhost' : '192.168.0.105';
const overmindPort = isDev ? '54108' : '5000';
const hubUrl = `http://${overmindHost}:${overmindPort}/robohub`;
```

## Обработчики команд

### 1. SendControlToRobo

Получает команды управления WASD от браузера.

**Формат данных:**
```json
{
  "roboId": "guid",
  "timeJs": 1234567890,
  "w": true,
  "s": false,
  "a": false,
  "d": true
}
```

**Обработка:**
- Преобразует WASD в команду для Arduino: `go(w,s,a,d);`
- Отправляет через serial порт

### 2. StartVideoStream

Запускает видеострим через ffmpeg.

**Формат данных:**
```json
{
  "udpAddress": "192.168.1.33:5000",
  "framerate": 15,
  "videoSize": "640x480"
}
```

**Параметры по умолчанию:**
- udpAddress: `192.168.1.33:5000`
- framerate: `15`
- videoSize: `640x480`

### 3. StopVideoStream

Останавливает активный видеострим.

## Автоматическое переподключение

- **Интервал**: 10 секунд
- **Логика**: 
  - При разрыве соединения автоматически пытается переподключиться
  - Использует встроенный механизм `withAutomaticReconnect`
  - Дополнительный таймер на случай полного отказа

## REST API для мониторинга

### GET /hub/status

Возвращает статус всех компонентов:

```json
{
  "signalr": {
    "state": "Connected",
    "connected": true,
    "roboId": "00000000-0000-0000-0000-000000000001"
  },
  "serial": {
    "isOpen": true,
    "path": "/dev/ttyUSB0"
  },
  "videoStream": {
    "active": false
  }
}
```

### POST /hub/test-invoke

Тестовый endpoint для вызова методов на сервере:

```bash
curl -X POST http://localhost:3000/hub/test-invoke \
  -H "Content-Type: application/json" \
  -d '{
    "method": "PushMessage",
    "data": {"message": "Test from robot"}
  }'
```

## Логирование

Все события логируются с префиксом `[Hub]`:

- `[Hub] Received control command` - получена команда управления
- `[Hub] Sending to serial` - отправка в serial порт
- `[Hub] Starting ffmpeg` - запуск видеострима
- `[SignalR] Connected to hub` - успешное подключение
- `[SignalR] Reconnecting...` - попытка переподключения

## Graceful Shutdown

При получении SIGINT (Ctrl+C):
1. Останавливает ffmpeg процесс (если активен)
2. Закрывает SignalR соединение
3. Завершает процесс

## Примеры использования

### Запуск робота

```bash
ROBO_ID=12345678-1234-1234-1234-123456789abc npm start
```

### Проверка статуса

```bash
curl http://localhost:3000/hub/status
```

### Отправка команды с браузера (через API)

```javascript
// В браузере через SignalR connection к /robohub
await connection.invoke('ReceiveControlFromBrowser', {
  roboId: '00000000-0000-0000-0000-000000000001',
  timeJs: Date.now(),
  w: true,
  s: false,
  a: false,
  d: false
});
```

### Запуск видеострима с браузера

```javascript
await connection.invoke('StartVideoStreamForRobo', 
  '00000000-0000-0000-0000-000000000001',
  {
    udpAddress: '192.168.1.33:5000',
    framerate: 30,
    videoSize: '1280x720'
  }
);
```

## Troubleshooting

### Робот не подключается к API

1. Проверьте доступность API: `curl http://<api-host>:<port>/robohub`
2. Проверьте логи: должно быть `[SignalR] Connected to hub`
3. Убедитесь, что `roboId` передается корректно

### Команды не доходят до робота

1. Проверьте статус: `GET /hub/status`
2. Убедитесь, что `signalr.connected === true`
3. Проверьте, что браузер использует правильный `roboId`

### Видеострим не запускается

1. Проверьте наличие ffmpeg: `which ffmpeg`
2. Проверьте доступность камеры: `ls /dev/video*`
3. Проверьте логи ffmpeg в консоли
