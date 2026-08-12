namespace GeSchool.Application.DTOs.CoursDtos;

public class CreateCoursDto
{
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semestre { get; set; } = string.Empty;
    public int DepartementId { get; set; }
    public int EnseignantId { get; set; }
}
