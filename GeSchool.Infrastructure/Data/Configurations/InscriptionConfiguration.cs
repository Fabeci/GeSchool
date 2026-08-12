using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class InscriptionConfiguration : IEntityTypeConfiguration<Inscription>
{
    public void Configure(EntityTypeBuilder<Inscription> builder)
    {
        builder.HasIndex(i => new { i.EtudiantId, i.CoursId }).IsUnique();

        builder.HasMany(i => i.Notes)
            .WithOne(n => n.Inscription)
            .HasForeignKey(n => n.InscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
