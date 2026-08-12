using FluentValidation;
using GeSchool.Application.DTOs.Etudiants;

namespace GeSchool.Application.Validators;

public class UpdateEtudiantDtoValidator : AbstractValidator<UpdateEtudiantDto>
{
    public UpdateEtudiantDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NumeroEtudiant).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prenom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DateNaissance).LessThan(DateTime.Today);
        RuleFor(x => x.DepartementId).GreaterThan(0);
    }
}
