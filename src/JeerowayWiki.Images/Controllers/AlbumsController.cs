using Atheneum.EntityImg;
using Atheneum.Enums;
using Atheneum.Services;
using Microsoft.AspNetCore.Mvc;
using JeerowayWiki.Images.Middleware;

namespace JeerowayWiki.Images.Controllers;

public class AlbumsController(AlbumsService service, ImgService imgService) : Controller
{
    public async Task<IActionResult> Index(AlbumEnum albumEnum = AlbumEnum.trash, CancellationToken ct = default)
    {
        var model = await service.List(albumEnum, ct);
        return View(model);
    }

    public async Task<IActionResult> Photo(Guid id, CancellationToken ct)
    {
        var photo = await imgService.Details(id, ct);
        return View(photo);
    }

    public async Task<IActionResult> Wiki(CancellationToken ct)
    {
        var model = await service.List(AlbumEnum.wiki, ct);
        return View(model);
    }

    public async Task<IActionResult> Show(int id, CancellationToken ct)
    {
        var model = await service.Details(id, ct);
        return View(model);
    }

    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Update(int? albumId, CancellationToken ct)
    {
        var model = albumId.HasValue ? await service.Details(albumId.Value, ct) : new Album();
        return View(model);
    }

    [HttpPost]
    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Update(Album album, CancellationToken ct)
    {
        var model = await service.Update(album, ct);
        return RedirectToAction(album.Type == AlbumEnum.wiki ? "Wiki" : "Index");
    }

    [HttpPost]
    [AuthorizeRoles(RoleEnum.imgManagement)]
    public async Task<IActionResult> Delete([FromQuery] int albumId, CancellationToken ct)
    {
        await service.Delete(albumId, ct);
        return RedirectToAction("Wiki");
    }
}