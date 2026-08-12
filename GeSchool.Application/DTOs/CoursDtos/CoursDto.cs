namespace GeSchool.Application.DTOs.CoursDtos;

public class CoursDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Semestre { get; set; } = string.Empty;
    public int DepartementId { get; set; }
    public int EnseignantId { get; set; }
}
