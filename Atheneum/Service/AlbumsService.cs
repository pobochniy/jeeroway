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
    public class AlbumsService : IAlbumsService
    {
        private ImagesContext db;

        public AlbumsService(ImagesContext context)
        {
            db = context;
        }

        public async Task Delete(int albumId)
        {
            var entity = await db.Albums.SingleAsync(x => x.Id == albumId);
            db.Albums.Remove(entity);
            await db.SaveChangesAsync();
        }

        public async Task<Album> Details(int albumId)
        {
            var res = await db.Albums.Include(x => x.Imgs).SingleAsync(x => x.Id == albumId);

            return res;
        }

        public async Task<IEnumerable<Album>> List(AlbumEnum type)
        {
            var res = await db.Albums
                .Where(x => x.Type == type)
                .ToArrayAsync();

            return res;
        }

        public async Task<Album> Update(Album album)
        {
            Album entity = null;
            if (album.Id == 0)
            {
                entity = new Album();
                await db.Albums.AddAsync(entity);
            }
            else
            {
                entity = await db.Albums.SingleAsync(x => x.Id == album.Id);
            }

            entity.Type = album.Type;
            entity.Name = album.Name;
            entity.Description = album.Description;
            entity.Created = DateTime.Now;

            await db.SaveChangesAsync();

            return entity;
        }
    }
}
