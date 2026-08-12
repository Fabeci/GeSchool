using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class DepartementConfiguration : IEntityTypeConfiguration<Departement>
{
    public void Configure(EntityTypeBuilder<Departement> builder)
    {
        builder.Property(d => d.Nom).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Description).HasMaxLength(1000);

        builder.HasMany(d => d.Enseignants)
            .WithOne(e => e.Departement)
            .HasForeignKey(e => e.DepartementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Etudiants)
            .WithOne(e => e.Departement)
            .HasForeignKey(e => e.DepartementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Cours)
            .WithOne(c => c.Departement)
            .HasForeignKey(c => c.DepartementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
