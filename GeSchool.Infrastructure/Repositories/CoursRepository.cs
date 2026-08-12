using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.Infrastructure.Repositories;

public class CoursRepository : Repository<Cours>, ICoursRepository
{
    public CoursRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cours?> GetByCodeAsync(string code) =>
        await DbSet.FirstOrDefaultAsync(c => c.Code == code);
}
