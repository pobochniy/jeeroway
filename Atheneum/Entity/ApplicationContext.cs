using Microsoft.EntityFrameworkCore;

namespace Atheneum.Entity.Identity
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserInRole> UserInRole { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new ProfileConfiguration());
            builder.ApplyConfiguration(new UserInRoleConfiguration());
        }
    }
}
