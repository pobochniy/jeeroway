using Atheneum.EntityImg;
using Atheneum.Enums;
using Microsoft.AspNetCore.Mvc;
using JeerowayWiki.Images.ModelView;
using JeerowayWiki.Images.Middleware;
using Atheneum.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace JeerowayWiki.Images.Controllers;

public class ImgController(IWebHostEnvironment env, ImgService service) : Controller
{
    private readonly string ext = "image/jpg";

    [ResponseCache(Duration = Int32.MaxValue)]
    public async Task<IActionResult> View(int albumId, Guid id, CancellationToken ct)
    {
        var pathToImg = Path.Combine(env.ContentRootPath, $"Content/Albums/{albumId}/{id}.jpg");

        if (!System.IO.File.Exists(pathToImg))
        {
            var res = await service.GetBinaryImg(id, ct);
            if (res.AlbumId != albumId) return RedirectToAction("View", new { albumId = res.AlbumId, id = id });
            var folder = new FileInfo(pathToImg);
            folder.Directory?.Create();

            using var image = Image.Load(res.ImgData.Bytes);
            
            if (image.Width > 1000)
            {
                var scale = 1000.0 / image.Width;
                var newWidth = 1000;
                var newHeight = (int)(image.Height * scale);
                
                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            await using var outputStream = new MemoryStream();
            await image.SaveAsJpegAsync(outputStream, ct);
            res.ImgData.Bytes = outputStream.ToArray();

            await System.IO.File.WriteAllBytesAsync(pathToImg, res.ImgData.Bytes, ct);
        }

        var stream = System.IO.File.OpenRead(pathToImg);
        return new FileStreamResult(stream, ext);
    }

    [AuthorizeRoles(RoleEnum.imgManagement)]
    public IActionResult Insert(int albumId, CancellationToken ct)
    {
        var model = new AddImg
        {
            AlbumId = albumId
        };
        return View(model);
    }

    [HttpPost]
    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Insert(AddImg addImg, CancellationToken ct)
    {
        if (addImg.AddImage != null && addImg.AddImage.Length > 0)
        {
            var dto = new Img
            {
                AlbumId = addImg.AlbumId,
                Title = addImg.Title,
                Description = addImg.Description
            };
            await service.Update(dto, GetFileBytes(addImg.AddImage), ct);
        }

        return RedirectToAction("Insert", new { albumId = addImg.AlbumId });
    }

    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Edit(Guid imgId, CancellationToken ct)
    {
        var model = await service.Details(imgId, ct);
        return View(model);
    }

    [HttpPost]
    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Edit(Img dto, CancellationToken ct)
    {
        await service.Update(dto, null!, ct);

        return RedirectToAction("Show", "Albums", new { id = dto.AlbumId });
    }

    [HttpGet]
    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Delete([FromQuery] Guid id, [FromQuery] int albumId, CancellationToken ct)
    {
        await service.Delete(id, ct);
        return RedirectToAction("Show", "Albums", new { id = albumId });
    }

    private static byte[] GetFileBytes(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        return fileBytes;
    }
}