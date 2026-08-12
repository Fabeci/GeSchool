using System.Reflection;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Departement> Departements => Set<Departement>();
    public DbSet<Enseignant> Enseignants => Set<Enseignant>();
    public DbSet<Etudiant> Etudiants => Set<Etudiant>();
    public DbSet<Cours> Cours => Set<Cours>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
