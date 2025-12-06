using Atheneum.EntityImg;
using Atheneum.Enums;
using Atheneum.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atheneum.Extentions.Auth;
using JeerowayWiki.Images.Middleware;

namespace JeerowayWiki.Images.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IAlbumsService service;
        private readonly IImgService imgService;

        public AlbumsController(IAlbumsService service, IImgService imgService)
        {
            this.service = service;
            this.imgService = imgService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await service.List(AlbumEnum.onsiteWasteCollection);
            return View(model);
        }

        public async Task<IActionResult> Photo(Guid id)
        {
            var photo = await imgService.Details(id);
            return View(photo);
        }

        public async Task<IActionResult> Wiki()
        {
            var model = await service.List(AlbumEnum.wiki);
            return View(model);
        }

        public async Task<IActionResult> Show(int id)
        {
            var model = await service.Details(id);
            return View(model);
        }

        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Update(int? albumId)
        {
            var model = albumId.HasValue ? await service.Details(albumId.Value) : new Album();
            return View(model);
        }

        [HttpPost]
        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Update(Album album)
        {
            var model = await service.Update(album);
            return RedirectToAction(album.Type == AlbumEnum.wiki ? "Wiki" : "Index");
        }

        [HttpPost]
        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Delete([FromQuery] int albumId)
        {
            await service.Delete(albumId);
            return RedirectToAction("Wiki");
        }
    }
}
