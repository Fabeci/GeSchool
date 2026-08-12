using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.Infrastructure.Repositories;

public class EtudiantRepository : Repository<Etudiant>, IEtudiantRepository
{
    public EtudiantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Etudiant?> GetByNumeroEtudiantAsync(string numeroEtudiant) =>
        await DbSet.FirstOrDefaultAsync(e => e.NumeroEtudiant == numeroEtudiant);
}
