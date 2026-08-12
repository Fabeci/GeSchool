namespace GeSchool.Application.DTOs.Departements;

public class UpdateDepartementDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
}
