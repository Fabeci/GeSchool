using System.ComponentModel.DataAnnotations;

namespace GeSchool.Application.DTOs.CoursDtos;

public class UpdateCoursDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semestre { get; set; } = string.Empty;

    [Display(Name = "Département")]
    public int DepartementId { get; set; }

    [Display(Name = "Enseignant")]
    public int EnseignantId { get; set; }
}
