using System.ComponentModel.DataAnnotations;
using GeSchool.Domain.Enums;

namespace GeSchool.Application.DTOs.Notes;

public class CreateNoteDto
{
    [Display(Name = "Inscription")]
    public int InscriptionId { get; set; }

    public decimal Valeur { get; set; }
    public TypeEvaluation TypeEvaluation { get; set; }
    public DateTime DateEvaluation { get; set; }
}
