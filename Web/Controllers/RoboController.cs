using Atheneum.Dto.Robo;
using Atheneum.Dto.User;
using Atheneum.Entity;
using Atheneum.Enums;
using Atheneum.Extentions.Auth;
using Atheneum.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Middleware;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class RoboController : Controller
    {
        private IRoboService service;

        public RoboController(IRoboService service)
        {
            this.service = service;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IEnumerable<RoboDto>> List()
        {
            var res = await service.List(User.GetUserId());
            return res;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<RoboDto> Details([FromQuery]Guid roboId)
        {
            var res = await service.Details(roboId);

            return res;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<RoboDto> Update([FromBody] RoboDto dto)
        {
            var res = await service.Update(dto, User.GetUserId());
            return res;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IEnumerable<RoboDto>> Delete([FromQuery] Guid roboId)
        {
            var res = await service.Delete(roboId, User.GetUserId());
            return res;
        }
    }
}
