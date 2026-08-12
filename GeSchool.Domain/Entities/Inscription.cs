using GeSchool.Domain.Common;

namespace GeSchool.Domain.Entities;

public class Inscription : BaseEntity
{
    public int EtudiantId { get; set; }
    public Etudiant Etudiant { get; set; } = null!;

    public int CoursId { get; set; }
    public Cours Cours { get; set; } = null!;

    public DateTime DateInscription { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
