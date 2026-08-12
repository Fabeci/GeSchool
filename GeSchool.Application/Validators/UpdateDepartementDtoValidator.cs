using FluentValidation;
using GeSchool.Application.DTOs.Departements;

namespace GeSchool.Application.Validators;

public class UpdateDepartementDtoValidator : AbstractValidator<UpdateDepartementDto>
{
    public UpdateDepartementDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(200);
    }
}
