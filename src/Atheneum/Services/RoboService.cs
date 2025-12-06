using Atheneum.Dto.Robo;
using Atheneum.Entity;
using Atheneum.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atheneum.Services;

public class RoboService(ApplicationContext context)
{
    public async Task<IEnumerable<RoboDto>> List(Guid userId)
    {
        var robots = await context.RoboMetadata
            .Where(x => x.MasterId == userId)
            .Select(x => new RoboDto
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
        var robot = await context.RoboMetadata.SingleAsync(x => x.Id == roboId);

        return robot.ToDto();
    }

    public async Task<RoboDto> Update(RoboDto dto, Guid currentUserId)
    {
        var res = !dto.Id.HasValue || dto.Id == Guid.Empty
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

        await context.RoboMetadata.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    private async Task<RoboMetadata> Edit(RoboDto dto, Guid currentUserId)
    {
        var entity = await context.RoboMetadata.SingleAsync(x => x.Id == dto.Id);
        if (entity.MasterId != currentUserId) throw new UnauthorizedAccessException();

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<RoboDto>> Delete(Guid roboId, Guid currentUserId)
    {
        var entity = await context.RoboMetadata.SingleAsync(x => x.Id == roboId);
        if (entity.MasterId != currentUserId) throw new UnauthorizedAccessException();

        context.RoboMetadata.Remove(entity);
        await context.SaveChangesAsync();

        return await List(currentUserId);
    }
}