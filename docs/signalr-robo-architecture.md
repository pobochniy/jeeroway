# SignalR RoboHub Architecture

## Обзор

API-сервер работает в двух режимах коммуникации через SignalR:

1. **Браузер → API** (клиент → сервер): браузер отправляет команды управления на API
2. **API → Робот** (сервер → клиент): API пересылает команды на робота (Node.js сервер)

Это два независимых SignalR-соединения с разными ролями клиент/сервер.

## Архитектура

```
┌─────────┐                    ┌─────────┐                    ┌────────┐
│ Browser │ ─── SignalR ────> │ API Hub │ ─── SignalR ────> │ Robot  │
│ Client  │   (connection 1)   │ Server  │   (connection 2)   │ Client │
└─────────┘                    └─────────┘                    └────────┘
```

### Connection 1: Browser → API
- **Клиент**: Angular приложение (браузер)
- **Сервер**: `RoboHub` в API
- **Направление**: Браузер вызывает серверные методы Hub

### Connection 2: API → Robot
- **Клиент**: Node.js сервер на роботе
- **Сервер**: `RoboHub` в API
- **Направление**: API вызывает клиентские методы через интерфейс `IRoboHub`

## Интерфейс IRoboHub

Определяет методы, которые **сервер вызывает на клиентах** (API → Robot):

```csharp
public interface IRoboHub
{
    /// <summary>
    /// Отправка команды управления с API на конкретного робота
    /// </summary>
    Task SendControlToRobo(RoboControlDto dto);
}
```

**Важно**: Один пользователь в одно время может подключиться только к одному роботу. Широковещательная рассылка не используется.

## Механизм групп SignalR

Для адресной доставки команд используются группы SignalR:

- **Формат группы**: `robo_{roboId}` (например, `robo_123e4567-e89b-12d3-a456-426614174000`)
- **Добавление в группу**: Происходит автоматически при подключении робота в `OnConnectedAsync()`
- **Отправка команд**: `Clients.Group(groupName).SendControlToRobo(dto)` отправляет команду только роботам в указанной группе
- **Удаление из группы**: Автоматически при отключении (SignalR управляет этим)

### Преимущества групп:
1. **Изоляция**: Команды доставляются только целевому роботу
2. **Масштабируемость**: Поддержка множества роботов без конфликтов
3. **Простота**: Не требуется ручное управление connectionId

## Класс RoboHub

### Управление подключениями и группами

#### `OnConnectedAsync()`
- **Назначение**: Обработка подключения клиента (браузер или робот)
- **Логика**: 
  - Извлекает `roboId` из query-параметра подключения
  - Добавляет подключение в группу `robo_{roboId}`
  - Логирует информацию о подключении
- **Группы**: Каждый робот добавляется в свою уникальную группу для адресной доставки команд

#### `OnDisconnectedAsync(Exception? exception)`
- **Назначение**: Обработка отключения клиента
- **Логика**: Логирует информацию об отключении

### Серверные методы (вызываются клиентами)

#### `ReceiveControlFromBrowser(RoboControlDto dto)`
- **Назначение**: Принимает команды управления от браузера
- **Вызывается**: Браузером через `connection.invoke("ReceiveControlFromBrowser", dto)`
- **Действие**: Валидирует данные и пересылает на робота через `ForwardControlToRobo`

#### `PushControl(RoboControlDto dto)` [Obsolete]
- **Назначение**: Устаревший метод для обратной совместимости
- **Рекомендация**: Использовать `ReceiveControlFromBrowser`

### Клиентские методы (вызываются сервером)

Определены в интерфейсе `IRoboHub`:
- `SendControlToRobo` - отправка команды на подключённого робота

## Поток данных

1. **Пользователь нажимает клавишу** в браузере
2. **Angular компонент** вызывает `connection.invoke("ReceiveControlFromBrowser", dto)`
3. **RoboHub.ReceiveControlFromBrowser** получает данные
4. **Валидация**: проверка dto, roboId, прав доступа
5. **ForwardControlToRobo** вызывает `Clients.Group($"robo_{dto.RoboId}").SendControlToRobo(dto)`
6. **SignalR** отправляет команду только роботу в группе `robo_{roboId}`
7. **Node.js клиент** на роботе получает команду через обработчик `SendControlToRobo`
8. **Опционально**: отправка мониторинга в ChatHub

## Пример использования

### Браузер (Angular)

```typescript
// Подключение к хабу (браузер НЕ добавляется в группу робота)
this.connection = new signalR.HubConnectionBuilder()
  .withUrl(`/hub/robocontrol?roboId=${this.roboId}`)
  .build();

await this.connection.start();

// Отправка команды
const dto = {
  roboId: this.roboId,
  timeJs: Date.now(),
  w: true,
  s: false,
  a: false,
  d: false
};

this.connection.invoke("ReceiveControlFromBrowser", dto)
  .catch(err => console.error(err));
```

### Робот (Node.js)

```javascript
const signalR = require("@microsoft/signalr");

// ВАЖНО: roboId должен быть передан в query-параметре для добавления в группу
const roboId = "your-robot-guid";
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`http://api-server/hub/robocontrol?roboId=${roboId}`)
  .build();

// Обработчик команд от сервера
connection.on("SendControlToRobo", (dto) => {
  console.log("Received control:", dto);
  // Обработка команды управления
  processRoboControl(dto);
});

await connection.start();
```

## Конфигурация

### Program.cs

```csharp
builder.Services.AddSignalR();

app.MapHub<RoboHub>("/hub/robocontrol");
```

## Безопасность

- **TODO**: Добавить `[Authorize]` атрибут к `ReceiveControlFromBrowser`
- **TODO**: Проверка принадлежности робота пользователю
- **TODO**: Rate limiting для предотвращения спама команд
- **TODO**: Аутентификация роботов через токены

## Мониторинг

Команды управления дублируются в `ChatHub` для мониторинга:
- Формат: JSON-сериализация `RoboControlDto`
- Тип сообщения: `ChatTypeEnum.text`
- Получатели: все подключённые к чату клиенты

## Troubleshooting

### Команды не доходят до робота
1. Проверить подключение робота к хабу
2. Проверить логи API: `SendControlToRobo invoked for RoboId=...`
3. Убедиться, что робот подписан на событие `SendControlToRobo`

### Браузер не может отправить команду
1. Проверить статус подключения: `isConnected` signal
2. Проверить наличие `roboId` в URL
3. Проверить логи браузера на ошибки SignalR

### Команды дублируются
- Проверить количество активных подключений робота к одной группе
- Убедиться, что робот передаёт корректный `roboId` при подключении
- Проверить логи: каждое подключение должно добавляться в уникальную группу `robo_{roboId}`
