using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;

namespace GeSchool.Infrastructure.Repositories;

public class EnseignantRepository : Repository<Enseignant>, IEnseignantRepository
{
    public EnseignantRepository(ApplicationDbContext context) : base(context)
    {
    }
}
