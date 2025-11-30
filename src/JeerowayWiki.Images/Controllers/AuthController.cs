using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atheneum.Dto.Auth;
using Atheneum.Interface;
using Atheneum.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JeerowayWiki.Images.Controllers;

[Route("[controller]")]
[AllowAnonymous]
public class AuthController : Controller
{
    private readonly AuthService service;

    public AuthController(AuthService service)
    {
        this.service = service;
    }

    [HttpGet("[action]")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(RegisterDto model, CancellationToken ct)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await service.Register(model, ct);
                var user = await service.LogIn(new LoginDto { Login = model.UserName, Password = model.Password }, ct);

                return RedirectToAction("Index", "Home");
            }

            return BadRequest(ModelState);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> LogIn(LoginDto model, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = await service.LogIn(model, ct);
                await SignInAsync(user);

                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "Неправильное имя пользователя или пароль");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        return BadRequest(ModelState);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInAsync(UserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, ((int)role).ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}