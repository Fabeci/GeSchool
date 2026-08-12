using FluentValidation;
using GeSchool.Application.DTOs.Notes;

namespace GeSchool.Application.Validators;

public class CreateNoteDtoValidator : AbstractValidator<CreateNoteDto>
{
    public CreateNoteDtoValidator()
    {
        RuleFor(x => x.InscriptionId).GreaterThan(0);
        RuleFor(x => x.Valeur).InclusiveBetween(0, 20);
    }
}
