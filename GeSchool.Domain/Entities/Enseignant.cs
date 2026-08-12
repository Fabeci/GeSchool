using GeSchool.Domain.Common;

namespace GeSchool.Domain.Entities;

public class Enseignant : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Specialite { get; set; }

    public int DepartementId { get; set; }
    public Departement Departement { get; set; } = null!;

    public ICollection<Cours> Cours { get; set; } = new List<Cours>();
}
