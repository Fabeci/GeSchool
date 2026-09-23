using System.ComponentModel.DataAnnotations;

namespace GeSchool.Application.DTOs.Etudiants;

public class CreateEtudiantDto
{
    public string NumeroEtudiant { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }

    [Display(Name = "Département")]
    public int DepartementId { get; set; }
}
