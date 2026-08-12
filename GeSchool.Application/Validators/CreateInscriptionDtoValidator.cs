using FluentValidation;
using GeSchool.Application.DTOs.Inscriptions;

namespace GeSchool.Application.Validators;

public class CreateInscriptionDtoValidator : AbstractValidator<CreateInscriptionDto>
{
    public CreateInscriptionDtoValidator()
    {
        RuleFor(x => x.EtudiantId).GreaterThan(0);
        RuleFor(x => x.CoursId).GreaterThan(0);
    }
}
