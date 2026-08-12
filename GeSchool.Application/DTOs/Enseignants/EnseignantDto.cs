namespace GeSchool.Application.DTOs.Enseignants;

public class EnseignantDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Specialite { get; set; }
    public int DepartementId { get; set; }
}
