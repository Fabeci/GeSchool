using GeSchool.Domain.Common;
using GeSchool.Domain.Enums;

namespace GeSchool.Domain.Entities;

public class Note : BaseEntity
{
    public int InscriptionId { get; set; }
    public Inscription Inscription { get; set; } = null!;

    public decimal Valeur { get; set; }
    public TypeEvaluation TypeEvaluation { get; set; }
    public DateTime DateEvaluation { get; set; }
}
