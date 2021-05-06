using Atheneum.EntityImg;
using Atheneum.Enums;
using Atheneum.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atheneum.Services
{
    public class ImgService : IImgService
    {
        private ImagesContext db;

        public ImgService(ImagesContext context)
        {
            db = context;
        }

        public async Task Delete(Guid imgId)
        {
            var entity = await db.Imgs.SingleOrDefaultAsync(x => x.Id == imgId);
            if (entity != null)
            {
                if (entity.Prev.HasValue || entity.Next.HasValue)
                {
                    var imgs = await db.Imgs.Where(x => x.AlbumId == entity.AlbumId).ToArrayAsync();
                    Img next = entity.Next.HasValue ? imgs.SingleOrDefault(x => x.Id == entity.Next) : null;
                    Img prev = entity.Prev.HasValue ? imgs.SingleOrDefault(x => x.Id == entity.Prev) : null;
                    if (next != null) next.Prev = prev?.Id;
                    if (prev != null) prev.Next = next?.Id;
                }

                db.ImgData.Remove(await db.ImgData.SingleAsync(x => x.Id == imgId));
                db.Imgs.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Img> Details(Guid imgId)
        {
            var res = await db.Imgs
                .Include(x => x.Album)
                .SingleAsync(x => x.Id == imgId);

            return res;
        }

        public async Task<Img> GetBinaryImg(Guid imgId)
        {
            var res = await db.Imgs.Include(x => x.ImgData).SingleAsync(x => x.Id == imgId);

            return res;
        }

        public async Task Update(Img img, byte[] data)
        {
            Album album = null;
            Img entity = null;
            Img prevImg = null;
            if (img.Id == Guid.Empty)
            {
                album = await db.Albums.Include(x => x.Imgs).SingleAsync(x => x.Id == img.AlbumId);
                prevImg = album.Imgs.SingleOrDefault(x => x.Next == null);

                entity = new Img
                {
                    AlbumId = img.AlbumId,
                    Created = DateTime.Now
                };

                if (prevImg != null) entity.Prev = prevImg.Id;

                entity.ImgData = new ImgData
                {
                    Bytes = data
                };

                await db.Imgs.AddAsync(entity);
            }
            else
            {
                entity = await db.Imgs.SingleAsync(x => x.Id == img.Id);
            }

            entity.Title = img.Title;
            entity.Description = img.Description;

            await db.SaveChangesAsync();

            if (prevImg != null)
            {
                prevImg.Next = entity.Id;
                await db.SaveChangesAsync();
            }

            if (album != null && album.PreviewImg == null)
            {
                album.PreviewImg = entity.Id;
                await db.SaveChangesAsync();
            }
        }
    }
}
