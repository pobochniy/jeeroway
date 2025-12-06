using System.Text.Json;
using Atheneum.Dto.Chat;
using Atheneum.Dto.Robo;
using Atheneum.Enums;
using Atheneum.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Jeeroway.Api.SignalR;

public class RoboHub(IHubContext<ChatHub, IChatHub> hubContext, ILogger<RoboHub> logger) : Hub<IRoboHub>
{
    private readonly IHubContext<ChatHub, IChatHub> _hub = hubContext;
    private readonly ILogger<RoboHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        var roboId = Context.GetHttpContext()?.Request.Query["roboId"].ToString();
        
        if (!string.IsNullOrEmpty(roboId))
        {
            // Добавляем подключение в группу робота
            await Groups.AddToGroupAsync(Context.ConnectionId, $"robo_{roboId}");
            _logger.LogInformation("Robot connected: RoboId={RoboId}, ConnectionId={ConnectionId}", roboId, Context.ConnectionId);
        }
        else
        {
            _logger.LogWarning("Connection without roboId: ConnectionId={ConnectionId}", Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roboId = Context.GetHttpContext()?.Request.Query["roboId"].ToString();
        
        if (!string.IsNullOrEmpty(roboId))
        {
            _logger.LogInformation("Robot disconnected: RoboId={RoboId}, ConnectionId={ConnectionId}", roboId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Серверный метод: принимает команду управления от браузера (клиент → сервер).
    /// Вызывается браузером через SignalR connection.invoke("ReceiveControlFromBrowser", dto).
    /// </summary>
    public async Task ReceiveControlFromBrowser(RoboControlDto dto)
    {
        try
        {
            if (dto == null)
            {
                _logger.LogError("ReceiveControlFromBrowser received null dto");
                throw new ArgumentNullException(nameof(dto));
            }

            _logger.LogInformation("ReceiveControlFromBrowser: RoboId={RoboId}, TimeJs={TimeJs}, W={W}, S={S}, A={A}, D={D}", 
                dto.RoboId, dto.TimeJs, dto.W, dto.S, dto.A, dto.D);
            
            if (dto.RoboId == Guid.Empty)
            {
                _logger.LogWarning("ReceiveControlFromBrowser received empty RoboId");
            }
            
            // TODO: проверка на принадлежность робота пользователю
            await ForwardControlToRobo(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ReceiveControlFromBrowser for RoboId={RoboId}", dto?.RoboId);
            throw;
        }
    }

    /// <summary>
    /// Пересылает команду управления на конкретного робота (сервер → клиент-робот).
    /// Использует группы SignalR для адресной доставки.
    /// </summary>
    private async Task ForwardControlToRobo(RoboControlDto msg)
    {
        try
        {
            var groupName = $"robo_{msg.RoboId}";
            
            // Отправка команды управления только конкретному роботу через группу
            await Clients.Group(groupName).SendControlToRobo(msg);
            _logger.LogDebug("SendControlToRobo invoked for RoboId={RoboId}, Group={GroupName}", msg.RoboId, groupName);
            
            // Опционально: отправка в чат для мониторинга
            // await SendMonitoringMessageToChat(msg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForwardControlToRobo for RoboId={RoboId}", msg.RoboId);
            throw;
        }
    }

    /// <summary>
    /// Серверный метод: принимает команду запуска видеострима от браузера (клиент → сервер).
    /// </summary>
    public async Task StartVideoStreamForRobo(Guid roboId, VideoStreamConfigDto? config = null)
    {
        try
        {
            _logger.LogInformation("StartVideoStreamForRobo: RoboId={RoboId}", roboId);
            
            var groupName = $"robo_{roboId}";
            config ??= new VideoStreamConfigDto();
            
            await Clients.Group(groupName).StartVideoStream(config);
            _logger.LogDebug("StartVideoStream invoked for RoboId={RoboId}", roboId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StartVideoStreamForRobo for RoboId={RoboId}", roboId);
            throw;
        }
    }

    /// <summary>
    /// Серверный метод: принимает команду остановки видеострима от браузера (клиент → сервер).
    /// </summary>
    public async Task StopVideoStreamForRobo(Guid roboId)
    {
        try
        {
            _logger.LogInformation("StopVideoStreamForRobo: RoboId={RoboId}", roboId);
            
            var groupName = $"robo_{roboId}";
            
            await Clients.Group(groupName).StopVideoStream();
            _logger.LogDebug("StopVideoStream invoked for RoboId={RoboId}", roboId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StopVideoStreamForRobo for RoboId={RoboId}", roboId);
            throw;
        }
    }

    /// <summary>
    /// Отправляет сообщение мониторинга в чат (опционально).
    /// </summary>
    // private async Task SendMonitoringMessageToChat(RoboControlDto msg)
    // {
    //     try
    //     {
    //         var txt = JsonSerializer.Serialize(msg);
    //         await _hub.Clients.All.BroadCastMessage(new ChatDto 
    //         { 
    //             Type = ChatTypeEnum.text, 
    //             Message = txt 
    //         });
    //         _logger.LogDebug("Chat monitoring message sent for RoboId={RoboId}", msg.RoboId);
    //     }
    //     catch (Exception chatEx)
    //     {
    //         // Не критично, если чат недоступен
    //         _logger.LogWarning(chatEx, "Failed to send chat message for RoboId={RoboId}, but control command was sent", msg.RoboId);
    //     }
    // }
}