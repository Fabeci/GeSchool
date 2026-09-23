using System.ComponentModel.DataAnnotations;

namespace GeSchool.Application.DTOs.Inscriptions;

public class UpdateInscriptionDto
{
    public int Id { get; set; }

    [Display(Name = "Étudiant")]
    public int EtudiantId { get; set; }

    [Display(Name = "Cours")]
    public int CoursId { get; set; }

    public DateTime DateInscription { get; set; }
}
