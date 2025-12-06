using Atheneum.Dto.Robo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Atheneum.Entity;

public class RoboMetadata
{
    /// <summary>
    /// Идентификатор робота, выдаётся при регистрации
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Наименование робота
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Краткое описание робота
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Идентификатор хозяина
    /// </summary>
    public Guid MasterId { get; set; }

    public virtual User User { get; set; }

    public RoboDto ToDto()
    {
        var res = new RoboDto
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            MasterId = this.MasterId
        };

        return res;
    }
}

public class RoboConfiguration : IEntityTypeConfiguration<RoboMetadata>
{
    public void Configure(EntityTypeBuilder<RoboMetadata> builder)
    {
        builder
            .HasOne(r => r.User)
            .WithMany(u => u.RoboMetadata)
            .HasForeignKey(u => u.MasterId);
    }
}