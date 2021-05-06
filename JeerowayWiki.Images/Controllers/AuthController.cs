using Atheneum.Dto.Auth;
using Atheneum.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService service;

        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpGet("[action]")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await service.Register(model);
                    var user = await service.LogIn(new LoginDto { Login = model.UserName, Password = model.Password });

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
        public async Task<IActionResult> LogIn(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await service.LogIn(model);
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
}