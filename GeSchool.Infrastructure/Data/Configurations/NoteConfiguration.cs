using GeSchool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeSchool.Infrastructure.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(n => n.Valeur).HasColumnType("decimal(4,2)");

        builder.ToTable(t => t.HasCheckConstraint("CK_Note_Valeur", "[Valeur] >= 0 AND [Valeur] <= 20"));
    }
}
