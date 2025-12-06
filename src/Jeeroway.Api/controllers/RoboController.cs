using Api.Middleware;
using Atheneum.Dto.Robo;
using Atheneum.Extentions.Auth;
using Atheneum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jeeroway.Api.controllers;

[Route("api/[controller]/[action]")]
[Authorize]
[ValidateRequest]
public class RoboController(RoboService service) : Controller
{
    [HttpGet]
    public async Task<IEnumerable<RoboDto>> List()
    {
        var res = await service.List(User.GetUserId());
        return res;
    }

    [HttpGet]
    public async Task<RoboDto> Details([FromQuery] Guid roboId)
    {
        var res = await service.Details(roboId);

        return res;
    }

    [HttpPost]
    public async Task<RoboDto> Update([FromBody] RoboDto dto)
    {
        var res = await service.Update(dto, User.GetUserId());
        return res;
    }

    [HttpGet]
    public async Task<IEnumerable<RoboDto>> Delete([FromQuery] Guid roboId)
    {
        var res = await service.Delete(roboId, User.GetUserId());
        return res;
    }
}