using FluentValidation;
using GeSchool.Application.DTOs.Departements;

namespace GeSchool.Application.Validators;

public class CreateDepartementDtoValidator : AbstractValidator<CreateDepartementDto>
{
    public CreateDepartementDtoValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(200);
    }
}
