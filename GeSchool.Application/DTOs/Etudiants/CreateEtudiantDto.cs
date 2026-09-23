using System.ComponentModel.DataAnnotations;
using GeSchool.Domain.Enums;

namespace GeSchool.Application.DTOs.Etudiants;

public class CreateEtudiantDto
{
    public string NumeroEtudiant { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }
    public Sexe? Sexe { get; set; }

    [Display(Name = "Département")]
    public int DepartementId { get; set; }
}
