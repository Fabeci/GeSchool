namespace GeSchool.Application.DTOs.Inscriptions;

public class CreateInscriptionDto
{
    public int EtudiantId { get; set; }
    public int CoursId { get; set; }
    public DateTime DateInscription { get; set; }
}
