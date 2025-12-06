using Atheneum.EntityImg;
using System;
using System.Threading.Tasks;

namespace Atheneum.Interface
{
    public interface IImgService
    {
        Task<Img> Details(Guid imgId);

        Task Update(Img img, byte[] data);

        Task Delete(Guid imgId);

        Task<Img> GetBinaryImg(Guid imgId);
    }
}
