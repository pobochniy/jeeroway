using Atheneum.Dto.Robo;
using Atheneum.Entity;
using Atheneum.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atheneum.Services
{
    public class RoboService : IRoboService
    {
        private readonly ApplicationContext db;

        public RoboService(ApplicationContext context)
        {
            db = context;
        }

        public async Task<IEnumerable<RoboDto>> List(Guid userId)
        {
            var robots = await db.RoboMetadata
                .Where(x => x.MasterId == userId)
                .Select(x=>new RoboDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    MasterId = x.MasterId
                })
                .ToArrayAsync();

            return robots;
        }

        public async Task<RoboDto> Details(Guid roboId)
        {
            var robot = await db.RoboMetadata.SingleAsync(x => x.Id == roboId);

            return robot.ToDto();
        }

        public async Task<RoboDto> Update(RoboDto dto, Guid currentUserId)
        {
            var res = dto.Id == Guid.Empty
                ? await Insert(dto, currentUserId)
                : await Edit(dto, currentUserId);

            return res.ToDto();
        }

        private async Task<RoboMetadata> Insert(RoboDto dto, Guid currentUserId)
        {
            var entity = new RoboMetadata
            {
                MasterId = currentUserId,
                Name = dto.Name,
                Description = dto.Description
            };

            await db.RoboMetadata.AddAsync(entity);
            await db.SaveChangesAsync();
            return entity;
        }

        private async Task<RoboMetadata> Edit(RoboDto dto, Guid currentUserId)
        {
            var entity = await db.RoboMetadata.SingleAsync(x => x.Id == dto.Id);
            if (entity.MasterId != currentUserId) throw new UnauthorizedAccessException();

            entity.Name = dto.Name;
            entity.Description = dto.Description;

            await db.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<RoboDto>> Delete(Guid roboId, Guid currentUserId)
        {
            var entity = await db.RoboMetadata.SingleAsync(x => x.Id == roboId);
            if (entity.MasterId != currentUserId) throw new UnauthorizedAccessException();

            db.RoboMetadata.Remove(entity);
            await db.SaveChangesAsync();

            return await List(currentUserId);
        }
    }
}
