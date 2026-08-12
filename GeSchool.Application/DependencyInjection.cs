using System.Reflection;
using FluentValidation;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeSchool.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IDepartementService, DepartementService>();
        services.AddScoped<IEnseignantService, EnseignantService>();
        services.AddScoped<IEtudiantService, EtudiantService>();
        services.AddScoped<ICoursService, CoursService>();
        services.AddScoped<IInscriptionService, InscriptionService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IBulletinService, BulletinService>();

        return services;
    }
}
