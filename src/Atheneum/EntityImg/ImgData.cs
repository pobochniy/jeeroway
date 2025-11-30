using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Atheneum.EntityImg
{
    public class ImgData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        public virtual Img Img { get; set; }
    }

    public class ImgDataConfiguration : IEntityTypeConfiguration<ImgData>
    {
        public void Configure(EntityTypeBuilder<ImgData> builder)
        {
            builder
                .HasOne(sc => sc.Img)
                .WithOne(s => s.ImgData)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasForeignKey<Img>(x => x.Id);
        }
    }
}
