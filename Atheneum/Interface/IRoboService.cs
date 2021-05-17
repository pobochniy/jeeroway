using Atheneum.Dto.Robo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atheneum.Interface
{
    public interface IRoboService
    {
        Task<IEnumerable<RoboDto>> List(Guid userId);

        Task<RoboDto> Details(Guid roboId);

        Task<RoboDto> Update(RoboDto dto, Guid currentUserId);

        Task<IEnumerable<RoboDto>> Delete(Guid roboId, Guid currentUserId);
    }
}
