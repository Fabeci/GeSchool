using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class EnseignantConfiguration : IEntityTypeConfiguration<Enseignant>
{
    public void Configure(EntityTypeBuilder<Enseignant> builder)
    {
        builder.Property(e => e.Nom).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Prenom).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Specialite).HasMaxLength(200);

        builder.HasMany(e => e.Cours)
            .WithOne(c => c.Enseignant)
            .HasForeignKey(c => c.EnseignantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
