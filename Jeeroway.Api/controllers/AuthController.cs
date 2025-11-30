using Atheneum.Dto.Auth;
using Atheneum.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
// using Api.Middleware;
// using Api.SignalR;
using Atheneum.Dto.Chat;
using Atheneum.Enums;
using Atheneum.Services;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

[Route("api/[controller]/[action]")]
[AllowAnonymous]
// [ValidateRequest]
public class AuthController : ControllerBase
{
    // private readonly IHubContext<ChatHub, IChatHub> _hub;
    private readonly AuthService _service;

    public AuthController(AuthService service) //, IHubContext<ChatHub, IChatHub> hubContext
    {
        _service = service;
        // _hub = hubContext;
    }

    [HttpPost]
    [Produces(typeof(UserDto))]
    public async Task<IActionResult> Register([FromBody] RegisterDto model, CancellationToken ct)
    {
        try
        {
            await _service.Register(model, ct);
            var user = await _service.LogIn(new LoginDto {Login = model.UserName, Password = model.Password}, ct);
            await SignInAsync(user);

            // await _hub.Clients.All.SysMessages(new SysMessagesDto {Type = ChatTypeEnum.user, Data = user});

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest($"{e.GetType()}: {e.Message}");
        }
    }

    [HttpPost]
    [Produces(typeof(UserDto))]
    public async Task<IActionResult> LogIn([FromBody]LoginDto model, CancellationToken ct)
    {
        try
        {
            var user = await _service.LogIn(model, ct);
            await SignInAsync(user);

            return Ok(user);
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError("", "Неправильное имя пользователя или пароль");
            return UnprocessableEntity(ModelState);
        }
        catch (Exception e)
        {
            return BadRequest($"{e.GetType()}: {e.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return new NoContentResult();
    }

    private async Task SignInAsync(UserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, ((int) role).ToString())));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}