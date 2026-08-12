using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeSchool.web.ViewModels;

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Rôle")]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "Lier à un étudiant (optionnel)")]
    public int? EtudiantId { get; set; }

    public IEnumerable<string> AvailableRoles { get; set; } = new List<string>();
    public IEnumerable<SelectListItem> AvailableEtudiants { get; set; } = new List<SelectListItem>();
}
