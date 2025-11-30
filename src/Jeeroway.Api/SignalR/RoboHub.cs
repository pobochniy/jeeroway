using System.Text.Json;
using Atheneum.Dto.Chat;
using Atheneum.Dto.Robo;
using Atheneum.Enums;
using Atheneum.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Jeeroway.Api.SignalR;

public class RoboHub : Hub<IRoboHub>
{
    private readonly IHubContext<ChatHub, IChatHub> _hub;

    public RoboHub(IHubContext<ChatHub, IChatHub> hubContext)
    {
        this._hub = hubContext;
    }

    //public override Task OnConnectedAsync()
    //{
    //    var username = Context.GetHttpContext().Request.Query["roboId"];
    //    // username = xxxx
    //    var kk = this.Clients;
    //    var kk2 = this.Clients.User(username);
    //    return base.OnConnectedAsync();
    //}

    //[Authorize]
    public async Task PushControl(RoboControlDto dto)
    {
        // TODO: проверка на принадлежность робота
        await SendPrivate(dto);
    }

    private async Task SendPrivate(RoboControlDto msg)
    {
        try
        {
            await this.Clients.All.BroadCastControl(msg);
            //await this.Clients.User(msg.RoboId.ToString()).BroadCastControl(msg);
            var txt = JsonSerializer.Serialize(msg);
            await _hub.Clients.All.BroadCastMessage(new ChatDto { Type = ChatTypeEnum.text, Message = txt });
        }
        catch
        {
        }
    }
}