using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.Infrastructure.Repositories;

public class InscriptionRepository : Repository<Inscription>, IInscriptionRepository
{
    public InscriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Inscription?> GetByEtudiantAndCoursAsync(int etudiantId, int coursId) =>
        await DbSet.FirstOrDefaultAsync(i => i.EtudiantId == etudiantId && i.CoursId == coursId);

    public async Task<IReadOnlyList<Inscription>> GetByEtudiantIdAsync(int etudiantId) =>
        await DbSet.Where(i => i.EtudiantId == etudiantId).ToListAsync();
}
