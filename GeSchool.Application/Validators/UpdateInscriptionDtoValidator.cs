using FluentValidation;
using GeSchool.Application.DTOs.Inscriptions;

namespace GeSchool.Application.Validators;

public class UpdateInscriptionDtoValidator : AbstractValidator<UpdateInscriptionDto>
{
    public UpdateInscriptionDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.EtudiantId).GreaterThan(0);
        RuleFor(x => x.CoursId).GreaterThan(0);
    }
}
