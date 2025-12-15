using Atheneum.EntityImg;
using Atheneum.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atheneum.Services;

public class AlbumsService(ImagesContext context)
{
    public async Task Delete(int albumId, CancellationToken ct)
    {
        var entity = await context.Albums.SingleAsync(x => x.Id == albumId, cancellationToken: ct);
        context.Albums.Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Album> Details(int albumId, CancellationToken ct)
    {
        var res = await context.Albums
            .Include(x => x.Imgs)
            .SingleAsync(x => x.Id == albumId, cancellationToken: ct);

        return res;
    }

    public async Task<IEnumerable<Album>> List(AlbumEnum type, CancellationToken ct)
    {
        var res = await context.Albums
            .Where(x => x.Type == type)
            .ToArrayAsync(cancellationToken: ct);

        return res;
    }

    public async Task<Album> Update(Album album, CancellationToken ct)
    {
        Album entity = null;
        if (album.Id == 0)
        {
            entity = new Album();
            await context.Albums.AddAsync(entity, ct);
        }
        else
        {
            entity = await context.Albums.SingleAsync(x => x.Id == album.Id, cancellationToken: ct);
        }

        entity.Type = album.Type;
        entity.Name = album.Name;
        entity.Description = album.Description;
        entity.Created = DateTime.Now;

        await context.SaveChangesAsync(ct);

        return entity;
    }
}