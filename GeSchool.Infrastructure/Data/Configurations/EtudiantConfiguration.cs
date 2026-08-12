using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class EtudiantConfiguration : IEntityTypeConfiguration<Etudiant>
{
    public void Configure(EntityTypeBuilder<Etudiant> builder)
    {
        builder.Property(e => e.NumeroEtudiant).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Nom).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Prenom).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);

        builder.HasIndex(e => e.NumeroEtudiant).IsUnique();

        builder.HasMany(e => e.Inscriptions)
            .WithOne(i => i.Etudiant)
            .HasForeignKey(i => i.EtudiantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
