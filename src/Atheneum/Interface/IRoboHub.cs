using Atheneum.Dto.Robo;

namespace Atheneum.Interface;

/// <summary>
/// Интерфейс для методов, которые сервер вызывает на клиентах RoboHub.
/// Определяет контракт для коммуникации API → Робот (клиент).
/// Один пользователь в одно время может подключиться только к одному роботу.
/// </summary>
public interface IRoboHub
{
    /// <summary>
    /// Отправка команды управления с API-сервера на робота.
    /// Вызывается сервером на клиенте-роботе через SignalR.
    /// </summary>
    /// <param name="dto">Данные управления роботом</param>
    Task SendControlToRobo(RoboControlDto dto);

    /// <summary>
    /// Команда запуска видеострима на роботе.
    /// </summary>
    /// <param name="config">Конфигурация видеострима</param>
    Task StartVideoStream(VideoStreamConfigDto config);

    /// <summary>
    /// Команда остановки видеострима на роботе.
    /// </summary>
    Task StopVideoStream();
}
