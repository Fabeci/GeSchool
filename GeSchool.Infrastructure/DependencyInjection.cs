using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Infrastructure.Data;
using GeSchool.Infrastructure.Identity;
using GeSchool.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeSchool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IDepartementRepository, DepartementRepository>();
        services.AddScoped<IEnseignantRepository, EnseignantRepository>();
        services.AddScoped<IEtudiantRepository, EtudiantRepository>();
        services.AddScoped<ICoursRepository, CoursRepository>();
        services.AddScoped<IInscriptionRepository, InscriptionRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();

        return services;
    }
}
