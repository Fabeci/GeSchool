using FluentValidation;
using GeSchool.Application.DTOs.Enseignants;

namespace GeSchool.Application.Validators;

public class CreateEnseignantDtoValidator : AbstractValidator<CreateEnseignantDto>
{
    public CreateEnseignantDtoValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prenom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DepartementId).GreaterThan(0);
    }
}
