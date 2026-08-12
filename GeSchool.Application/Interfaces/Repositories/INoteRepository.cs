using GeSchool.Domain.Entities;

namespace GeSchool.Application.Interfaces.Repositories;

public interface INoteRepository : IRepository<Note>
{
    Task<IReadOnlyList<Note>> GetByInscriptionIdAsync(int inscriptionId);
}
