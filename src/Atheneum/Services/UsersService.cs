using Atheneum.Entity;
using Atheneum.Enums;
using Atheneum.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atheneum.Services;

public class UsersService(ApplicationContext context)
{
    public async Task<IEnumerable<Profile>> GetProfiles()
    {
        var res = await context.Profiles.ToArrayAsync();

        return res;
    }

    public async Task<IEnumerable<RoleEnum>> GetRoles(Guid userId)
    {
        var query = context.UserInRole
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId);

        var res = await query.ToArrayAsync();
        return res;
    }

    public async Task SetRoles(Guid userId, IEnumerable<RoleEnum> roles)
    {
        var currentRoles = await context.UserInRole
            .Where(x => x.UserId == userId)
            .ToArrayAsync();

        context.UserInRole.RemoveRange(currentRoles);
        await context.SaveChangesAsync();

        foreach (var role in roles)
        {
            var userInRole = new UserInRole
            {
                UserId = userId,
                RoleId = role
            };

            context.UserInRole.Add(userInRole);
        }

        await context.SaveChangesAsync();
    }
}