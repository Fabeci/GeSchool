using GeSchool.Domain.Common;

namespace GeSchool.Domain.Entities;

public class Departement : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Enseignant> Enseignants { get; set; } = new List<Enseignant>();
    public ICollection<Etudiant> Etudiants { get; set; } = new List<Etudiant>();
    public ICollection<Cours> Cours { get; set; } = new List<Cours>();
}
