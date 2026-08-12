using FluentValidation;
using GeSchool.Application.DTOs.Notes;

namespace GeSchool.Application.Validators;

public class UpdateNoteDtoValidator : AbstractValidator<UpdateNoteDto>
{
    public UpdateNoteDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.InscriptionId).GreaterThan(0);
        RuleFor(x => x.Valeur).InclusiveBetween(0, 20);
    }
}
