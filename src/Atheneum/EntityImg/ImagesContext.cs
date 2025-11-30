using Microsoft.EntityFrameworkCore;

namespace Atheneum.EntityImg
{
    public class ImagesContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Img> Imgs { get; set; }
        public DbSet<ImgData> ImgData { get; set; }

        public ImagesContext(DbContextOptions<ImagesContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ImgConfiguration());
            builder.ApplyConfiguration(new ImgDataConfiguration());
        }
    }
}
