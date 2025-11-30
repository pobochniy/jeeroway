using Atheneum.EntityImg;
using Atheneum.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atheneum.Interface
{
    public interface IAlbumsService
    {
        Task<IEnumerable<Album>> List(AlbumEnum type);

        Task<Album> Details(int albumId);

        Task<Album> Update(Album album);

        Task Delete(int albumId);
    }
}
