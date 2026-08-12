namespace GeSchool.Application.DTOs.Notes;

public class BulletinDto
{
    public int EtudiantId { get; set; }
    public string NumeroEtudiant { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public IReadOnlyList<BulletinCoursDto> Cours { get; set; } = new List<BulletinCoursDto>();
    public decimal MoyenneGenerale { get; set; }
}
