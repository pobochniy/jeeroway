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
using JeerowayWiki.Images.ModelView;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using JeerowayWiki.Images.Middleware;
using System.Drawing;

namespace JeerowayWiki.Images.Controllers
{
    public class ImgController : Controller
    {
        private readonly string ext = "image/jpg";
        private readonly IWebHostEnvironment env;
        private readonly IImgService service;

        public ImgController(IWebHostEnvironment env, IImgService service)
        {
            this.env = env;
            this.service = service;
        }

        [ResponseCache(Duration = Int32.MaxValue)]
        public async Task<IActionResult> View(int albumId, Guid id)
        {
            string pathToImg = Path.Combine(env.ContentRootPath, $"Content/Albums/{albumId}/{id}.jpg");

            if (!System.IO.File.Exists(pathToImg))
            {
                var res = await service.GetBinaryImg(id);
                if (res.AlbumId != albumId) return RedirectToAction("View", new { albumId = res.AlbumId, id = id });
                var folder = new FileInfo(pathToImg);
                folder.Directory.Create();

                MemoryStream inputMemoryStream = new MemoryStream(res.ImgData.Bytes);
                Image fullsizeImage = Image.FromStream(inputMemoryStream);
                if ((int)fullsizeImage.Width > 1000)
                {
                    var scale = (decimal)1000 / (int)fullsizeImage.Width;
                    Bitmap fullSizeBitmap = new Bitmap(fullsizeImage, new Size((int)(fullsizeImage.Width * scale), (int)(fullsizeImage.Height * scale)));
                    MemoryStream resultStream = new MemoryStream();

                    fullSizeBitmap.Save(resultStream, fullsizeImage.RawFormat);
                    res.ImgData.Bytes = resultStream.ToArray();

                    resultStream.Dispose();
                    resultStream.Close();
                }

                System.IO.File.WriteAllBytes(pathToImg, res.ImgData.Bytes);
            }

            var stream = System.IO.File.OpenRead(pathToImg);
            return new FileStreamResult(stream, ext);
        }

        [AuthorizeRoles(RoleEnum.imgManagement)]
        public IActionResult Insert(int albumId)
        {
            var model = new AddImg
            {
                AlbumId = albumId
            };
            return View(model);
        }

        [HttpPost]
        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Insert(AddImg addImg)
        {
            if (addImg.AddImage != null && addImg.AddImage.Length > 0)
            {
                var dto = new Img
                {
                    AlbumId = addImg.AlbumId,
                    Title = addImg.Title,
                    Description = addImg.Description
                };
                await service.Update(dto, GetFileBytes(addImg.AddImage));
            }

            return RedirectToAction("Insert", new { albumId = addImg.AlbumId });
        }

        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Edit(Guid imgId)
        {
            var model = await service.Details(imgId);
            return View(model);
        }

        [HttpPost]
        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Edit(Img dto)
        {
            await service.Update(dto, null);
            
            return RedirectToAction("Show", "Albums", new { id = dto.AlbumId });
        }

        [HttpGet]
        [AuthorizeRoles(RoleEnum.imgManagement)]
        public async Task<IActionResult> Delete([FromQuery] Guid id, [FromQuery] int albumId)
        {
            await service.Delete(id);
            return RedirectToAction("Show", "Albums", new { id = albumId });
        }

        private byte[] GetFileBytes(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                return fileBytes;
            }
        }
    }
}
