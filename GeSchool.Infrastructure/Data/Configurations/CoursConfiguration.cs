using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class CoursConfiguration : IEntityTypeConfiguration<Cours>
{
    public void Configure(EntityTypeBuilder<Cours> builder)
    {
        builder.Property(c => c.Code).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Intitule).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Semestre).IsRequired().HasMaxLength(20);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint("CK_Cours_Credits", "[Credits] > 0"));

        builder.HasMany(c => c.Inscriptions)
            .WithOne(i => i.Cours)
            .HasForeignKey(i => i.CoursId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
