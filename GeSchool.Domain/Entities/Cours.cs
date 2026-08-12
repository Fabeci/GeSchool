using GeSchool.Domain.Common;

namespace GeSchool.Domain.Entities;

public class Cours : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semestre { get; set; } = string.Empty;

    public int DepartementId { get; set; }
    public Departement Departement { get; set; } = null!;

    public int EnseignantId { get; set; }
    public Enseignant Enseignant { get; set; } = null!;

    public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
}
