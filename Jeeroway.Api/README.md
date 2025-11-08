# Jeeroway.Api

Легкий ASP.NET Core (NET 8) API-сервис для приема JPEG-кадров по UDP и записи их на диск в рамках сессий. Управление записью и доступ к кадрам — через REST эндпоинты и Swagger UI.

- **Прием кадров по UDP**: сервис слушает порт `5000`, собирает поток байт и вырезает кадры по маркерам JPEG `FFD8 ... FFD9`.
- **Сессии записи**: старт/стоп сессии; кадры сохраняются в папку `recordings/<session-id>/` как `frame_000000.jpg`, `frame_000001.jpg`, ...
- **REST API**: контроллер `VideoStreamController` под маршрутом `/video` для управления и выдачи кадров.
- **Swagger UI**: документация и тестирование API из браузера.
- **Отдельный тестовый UDP-сервис**: `UdpListener` слушает порт `5175` и пишет входящие датаграммы как JPEG в каталог (по умолчанию жестко задан: `/Users/viktorpobochniy/webcam/`).

## Основная функциональность

- **[UdpVideoReceiverService]** — читает UDP (порт 5000), буферизует поток и выделяет JPEG-кадры.
- **[RecordingSessionManager]** — управляет сессиями записи и сохранением кадров на диск.
- **[VideoStreamController]** — REST API для управления сессиями и доступа к кадрам.
- **[Swagger]** — интерактивная документация API.
- **[UdpListener]** — альтернативный/тестовый приемник на 5175, сохраняет каждую датаграмму в файл JPEG.

## Архитектура и файлы

- `Program.cs` — DI и запуск приложения:
  - Регистрация `Channel<byte[]>`, `RecordingSessionManager`, Hosted Services `UdpVideoReceiverService`, `UdpListener`.
  - `AddControllers()`, `AddSwaggerGen()`.
- `UdpVideoReceiverService.cs` — фоновой сервис:
  - UDP порт `5000` (`UdpClient(5000)`), сборка JPEG по SOI/EOI.
  - Пишет кадр в канал и вызывает `RecordingSessionManager.SaveFrame(frame)`.
- `RecordingSessionManager.cs` — управление сессией:
  - `StartSession()` -> `recordings/session-YYYY-MM-DD_HH-mm-ss/`.
  - `StopSession()` обнуляет состояние;
  - `SaveFrame(byte[])`, `ListSessions()`, `ListFrames(sessionId)`, `GetFramePath(...)`.
- `controllers/VideoStreamController.cs` — REST:
  - `POST /video/start-recording`, `POST /video/stop-recording`.
  - `GET /video/sessions`, `GET /video/sessions/{sessionId}`.
  - `GET /video/sessions/{sid}/frames/{file}` -> `image/jpeg`.
- `UdpListener.cs` — тестовый приемник на `5175`, пишет файлы в `/Users/viktorpobochniy/webcam/`.
- `recordings/` — каталог хранения записей.

## Быстрый старт

Требования: .NET SDK 8.0+

Запуск:

```bash
# из корня репозитория либо из каталога проекта
 dotnet run --project Jeeroway.Api/Jeeroway.Api.csproj
```

Swagger UI:

- Откройте URL, который напечатает Kestrel при старте (например, http://localhost:5xxx/swagger).
- Либо задайте порт явно:

```bash
 ASPNETCORE_URLS=http://localhost:8080 dotnet run --project Jeeroway.Api/Jeeroway.Api.csproj
# Swagger: http://localhost:8080/swagger
```

## REST API

- `POST /video/start-recording`
  - Ответ: `{ "sessionId": "session-YYYY-MM-DD_HH-mm-ss" }`
- `POST /video/stop-recording`
  - Ответ: `"Recording stopped"`
- `GET /video/sessions`
  - Ответ: `["session-2025-...", ...]`
- `GET /video/sessions/{sessionId}`
  - Ответ: `["frame_000000.jpg", "frame_000001.jpg", ...]`
- `GET /video/sessions/{sessionId}/frames/{fileName}`
  - Ответ: `image/jpeg` (файл кадра)

Примеры curl (порт подставьте свой):

```bash
# старт записи
 curl -X POST http://localhost:8080/video/start-recording

# список сессий
 curl http://localhost:8080/video/sessions

# список кадров в сессии
 curl http://localhost:8080/video/sessions/<sessionId>

# скачать кадр
 curl -o frame.jpg \
  http://localhost:8080/video/sessions/<sessionId>/frames/frame_000000.jpg

# стоп записи
 curl -X POST http://localhost:8080/video/stop-recording
```

## Отправка JPEG по UDP (пример отправителя)

Сервис на стороне сервера собирает кадры по маркерам JPEG, поэтому кадр может быть разбит на несколько датаграмм. Рекомендуется слать чанками ~1400 байт, чтобы не упираться в MTU.

### Python

```python
import socket

HOST = "127.0.0.1"
PORT = 5000  # UdpVideoReceiverService
CHUNK = 1400

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

with open("frame.jpg", "rb") as f:
    data = f.read()
    for i in range(0, len(data), CHUNK):
        sock.sendto(data[i:i+CHUNK], (HOST, PORT))
```

### C# (.NET)

```csharp
using System.Net.Sockets;

var host = "127.0.0.1";
var port = 5000;
var chunk = 1400;

using var client = new UdpClient();
var data = await File.ReadAllBytesAsync("frame.jpg");
for (var i = 0; i < data.Length; i += chunk)
{
    var size = Math.Min(chunk, data.Length - i);
    await client.SendAsync(data.AsMemory(i, size), host, port);
}
```

Примечание: если вы хотите писать кадры в сессию, обязательно сначала вызовите `POST /video/start-recording`. Без активной сессии `SaveFrame` пропускает сохранение.

## Структура хранения

```
Jeeroway.Api/
  recordings/
    session-2025-11-06_18-10-00/
      frame_000000.jpg
      frame_000001.jpg
      ...
```

## Известные ограничения и заметки

- `UdpListener` (порт 5175) пишет файлы в жестко заданный путь `/Users/viktorpobochniy/webcam/`. Измените путь/порт в `UdpListener.cs`, если требуется.
- Канал `Channel<byte[]>` пока не используется контроллером для живого стриминга, но зарезервирован для будущих задач.
- Файлы пишутся синхронно (`File.WriteAllBytes`), для больших нагрузок можно заменить на асинхронную запись и добавить очередь/ограничение скорости.
- Управление сессией простое и не рассчитано на одновременные множественные сессии; текущая активна максимум одна.
