using GeSchool.Domain.Common;

namespace GeSchool.Domain.Entities;

public class Etudiant : BaseEntity
{
    public string NumeroEtudiant { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }

    public int DepartementId { get; set; }
    public Departement Departement { get; set; } = null!;

    public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
}
