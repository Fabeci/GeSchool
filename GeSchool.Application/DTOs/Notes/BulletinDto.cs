using GeSchool.Domain.Enums;

namespace GeSchool.Application.DTOs.Notes;

public class BulletinDto
{
    public int EtudiantId { get; set; }
    public string NumeroEtudiant { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public Sexe? Sexe { get; set; }
    public string Departement { get; set; } = string.Empty;
    public IReadOnlyList<BulletinCoursDto> Cours { get; set; } = new List<BulletinCoursDto>();
    public decimal MoyenneGenerale { get; set; }
}
