using System.ComponentModel.DataAnnotations;

namespace GeSchool.Application.DTOs.Enseignants;

public class UpdateEnseignantDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Specialite { get; set; }

    [Display(Name = "Département")]
    public int DepartementId { get; set; }
}
