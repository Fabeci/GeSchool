using FluentValidation;
using GeSchool.Application.DTOs.CoursDtos;

namespace GeSchool.Application.Validators;

public class UpdateCoursDtoValidator : AbstractValidator<UpdateCoursDto>
{
    public UpdateCoursDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Intitule).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Credits).GreaterThan(0);
        RuleFor(x => x.Semestre).NotEmpty().MaximumLength(20);
        RuleFor(x => x.DepartementId).GreaterThan(0);
        RuleFor(x => x.EnseignantId).GreaterThan(0);
    }
}
