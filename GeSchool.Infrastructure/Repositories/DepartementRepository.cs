using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Domain.Entities;
using GeSchool.Infrastructure.Data;

namespace GeSchool.Infrastructure.Repositories;

public class DepartementRepository : Repository<Departement>, IDepartementRepository
{
    public DepartementRepository(ApplicationDbContext context) : base(context)
    {
    }
}
