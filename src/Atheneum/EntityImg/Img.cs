using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atheneum.EntityImg
{
    public class Img
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int AlbumId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public Guid? Prev { get; set; }

        public Guid? Next { get; set; }

        public virtual Album Album { get; set; }

        public virtual ImgData ImgData { get; set; }
    }

    public class ImgConfiguration : IEntityTypeConfiguration<Img>
    {
        public void Configure(EntityTypeBuilder<Img> builder)
        {
            builder
                .HasOne(sc => sc.Album)
                .WithMany(s => s.Imgs)
                .HasForeignKey(sc => sc.AlbumId);
        }
    }
}
