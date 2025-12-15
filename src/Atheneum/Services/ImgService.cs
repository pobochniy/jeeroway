using Atheneum.EntityImg;
using Microsoft.EntityFrameworkCore;

namespace Atheneum.Services;

public class ImgService(ImagesContext context)
{
    public async Task Delete(Guid imgId, CancellationToken ct)
    {
        var entity = await context.Imgs.SingleOrDefaultAsync(x => x.Id == imgId, cancellationToken: ct);
        if (entity != null)
        {
            if (entity.Prev.HasValue || entity.Next.HasValue)
            {
                var imgs = await context.Imgs
                    .Where(x => x.AlbumId == entity.AlbumId)
                    .ToArrayAsync(cancellationToken: ct);
                var next = entity.Next.HasValue ? imgs.SingleOrDefault(x => x.Id == entity.Next) : null;
                var prev = entity.Prev.HasValue ? imgs.SingleOrDefault(x => x.Id == entity.Prev) : null;
                if (next != null) next.Prev = prev?.Id;
                if (prev != null) prev.Next = next?.Id;
            }

            context.ImgData.Remove(await context.ImgData.SingleAsync(x => x.Id == imgId, cancellationToken: ct));
            context.Imgs.Remove(entity);
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task<Img> Details(Guid imgId, CancellationToken ct)
    {
        var res = await context.Imgs
            .Include(x => x.Album)
            .SingleAsync(x => x.Id == imgId, cancellationToken: ct);

        return res;
    }

    public async Task<Img> GetBinaryImg(Guid imgId, CancellationToken ct)
    {
        var res = await context.Imgs
            .Include(x => x.ImgData)
            .SingleAsync(x => x.Id == imgId, cancellationToken: ct);

        return res;
    }

    public async Task Update(Img img, byte[] data, CancellationToken ct)
    {
        Album album = null;
        Img entity = null;
        Img prevImg = null;
        if (img.Id == Guid.Empty)
        {
            album = await context.Albums.Include(x => x.Imgs)
                .SingleAsync(x => x.Id == img.AlbumId, cancellationToken: ct);
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

            await context.Imgs.AddAsync(entity, ct);
        }
        else
        {
            entity = await context.Imgs.SingleAsync(x => x.Id == img.Id, cancellationToken: ct);
        }

        entity.Title = img.Title;
        entity.Description = img.Description;

        await context.SaveChangesAsync(ct);

        if (prevImg != null)
        {
            prevImg.Next = entity.Id;
            await context.SaveChangesAsync(ct);
        }

        if (album != null && album.PreviewImg == null)
        {
            album.PreviewImg = entity.Id;
            await context.SaveChangesAsync(ct);
        }
    }
}