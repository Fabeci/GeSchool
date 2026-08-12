namespace GeSchool.Application.DTOs.Inscriptions;

public class UpdateInscriptionDto
{
    public int Id { get; set; }
    public int EtudiantId { get; set; }
    public int CoursId { get; set; }
    public DateTime DateInscription { get; set; }
}
