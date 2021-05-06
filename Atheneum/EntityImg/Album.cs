using Atheneum.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atheneum.EntityImg
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        public AlbumEnum Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public Guid? PreviewImg { get; set; }

        public virtual ICollection<Img> Imgs { get; set; }

        public Album()
        {
            Imgs = new List<Img>();
        }
    }
}
