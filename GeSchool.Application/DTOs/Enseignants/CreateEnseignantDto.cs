using System.ComponentModel.DataAnnotations;

namespace GeSchool.Application.DTOs.Enseignants;

public class CreateEnseignantDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Specialite { get; set; }

    [Display(Name = "Département")]
    public int DepartementId { get; set; }
}
