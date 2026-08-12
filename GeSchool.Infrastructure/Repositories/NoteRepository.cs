using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.Infrastructure.Repositories;

public class NoteRepository : Repository<Note>, INoteRepository
{
    public NoteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Note>> GetByInscriptionIdAsync(int inscriptionId) =>
        await DbSet.Where(n => n.InscriptionId == inscriptionId).ToListAsync();
}
