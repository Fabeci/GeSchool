namespace GeSchool.Application.DTOs.Notes;

public class BulletinCoursDto
{
    public int CoursId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public int Credits { get; set; }
    public IReadOnlyList<NoteDto> Notes { get; set; } = new List<NoteDto>();
    public decimal? MoyenneCours { get; set; }
}
