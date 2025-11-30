using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace JeerowayWiki.Images.ModelView
{
    public class AddImg
    {
        public Guid Id { get; set; } = Guid.Empty;

        [Required]
        public int AlbumId { get; set; }

        [Required]
        public IFormFile AddImage { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
