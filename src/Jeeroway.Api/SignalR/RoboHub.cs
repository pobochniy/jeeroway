using Atheneum.Dto.Robo;
using Atheneum.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Jeeroway.Api.SignalR;

public class RoboHub(
    IHubContext<ChatHub, IChatHub> hubContext, 
    ILogger<RoboHub> logger,
    UdpVideoReceiverService videoReceiver) : Hub<IRoboHub>
{
    private readonly IHubContext<ChatHub, IChatHub> _hub = hubContext;
    private readonly ILogger<RoboHub> _logger = logger;
    private readonly UdpVideoReceiverService _videoReceiver = videoReceiver;

    public override async Task OnConnectedAsync()
    {
        var roboIdStr = Context.GetHttpContext()?.Request.Query["roboId"].ToString();
        
        if (!string.IsNullOrEmpty(roboIdStr) && Guid.TryParse(roboIdStr, out var roboId))
        {
            // Добавляем подключение в группу робота
            await Groups.AddToGroupAsync(Context.ConnectionId, $"robo_{roboId}");
            
            // Регистрируем IP адрес робота для видеострима
            var ipAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress;
            if (ipAddress != null)
            {
                _videoReceiver.RegisterRobot(ipAddress, roboId);
                _logger.LogInformation("Robot connected: RoboId={RoboId}, ConnectionId={ConnectionId}, IP={IpAddress}", 
                    roboId, Context.ConnectionId, ipAddress);
            }
            else
            {
                _logger.LogWarning("Robot connected without IP: RoboId={RoboId}, ConnectionId={ConnectionId}", 
                    roboId, Context.ConnectionId);
            }
        }
        else
        {
            _logger.LogWarning("Connection without valid roboId: ConnectionId={ConnectionId}", Context.ConnectionId);
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
    public async Task StartVideoStreamForRobo(string roboIdStr)
    {
        try
        {
            if (!Guid.TryParse(roboIdStr, out var roboId))
            {
                _logger.LogError("Invalid roboId format: {RoboIdStr}", roboIdStr);
                throw new ArgumentException($"Invalid roboId format: {roboIdStr}");
            }

            _logger.LogInformation("StartVideoStreamForRobo: RoboId={RoboId}", roboId);
            
            var groupName = $"robo_{roboId}";
            var config = new VideoStreamConfigDto();
            
            await Clients.Group(groupName).StartVideoStream(config);
            _logger.LogDebug("StartVideoStream invoked for RoboId={RoboId}", roboId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StartVideoStreamForRobo for RoboId={RoboIdStr}", roboIdStr);
            throw;
        }
    }

    /// <summary>
    /// Серверный метод: принимает команду остановки видеострима от браузера (клиент → сервер).
    /// </summary>
    public async Task StopVideoStreamForRobo(string roboIdStr)
    {
        try
        {
            if (!Guid.TryParse(roboIdStr, out var roboId))
            {
                _logger.LogError("Invalid roboId format: {RoboIdStr}", roboIdStr);
                throw new ArgumentException($"Invalid roboId format: {roboIdStr}");
            }

            _logger.LogInformation("StopVideoStreamForRobo: RoboId={RoboId}", roboId);
            
            var groupName = $"robo_{roboId}";
            
            await Clients.Group(groupName).StopVideoStream();
            _logger.LogDebug("StopVideoStream invoked for RoboId={RoboId}", roboId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in StopVideoStreamForRobo for RoboId={RoboIdStr}", roboIdStr);
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