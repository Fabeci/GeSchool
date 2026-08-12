using GeSchool.Domain.Enums;

namespace GeSchool.Application.DTOs.Notes;

public class NoteDto
{
    public int Id { get; set; }
    public int InscriptionId { get; set; }
    public decimal Valeur { get; set; }
    public TypeEvaluation TypeEvaluation { get; set; }
    public DateTime DateEvaluation { get; set; }
}
