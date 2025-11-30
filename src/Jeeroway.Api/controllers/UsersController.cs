using Atheneum.Dto.User;
using Atheneum.Entity;
using Atheneum.Enums;
using Atheneum.Interface;
using Atheneum.Services;
using Jeeroway.Api.middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jeeroway.Api.controllers;

[Route("api/[controller]")]
public class UsersController : Controller
{
    private UsersService service;

    public UsersController(UsersService service)
    {
        this.service = service;
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public async Task<IEnumerable<Profile>> GetProfiles()
    {
        var res = await service.GetProfiles();
        return res;
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public async Task<IEnumerable<RoleEnum>> GetRoles(Guid userId)
    {
        var res = await service.GetRoles(userId);

        return res;
    }

    [HttpPost]
    [Route("[action]")]
    [AuthorizeRoles(RoleEnum.roleManagement)]
    public async Task SetRoles([FromBody] UserAndRolesDto dto)
    {
        await service.SetRoles(dto.UserId, dto.Roles);
    }
}