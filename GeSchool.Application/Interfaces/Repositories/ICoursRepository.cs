using GeSchool.Domain.Entities;

namespace GeSchool.Application.Interfaces.Repositories;

public interface ICoursRepository : IRepository<Cours>
{
    Task<Cours?> GetByCodeAsync(string code);
}
