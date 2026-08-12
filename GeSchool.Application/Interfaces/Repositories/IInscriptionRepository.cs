using GeSchool.Domain.Entities;

namespace GeSchool.Application.Interfaces.Repositories;

public interface IInscriptionRepository : IRepository<Inscription>
{
    Task<Inscription?> GetByEtudiantAndCoursAsync(int etudiantId, int coursId);
    Task<IReadOnlyList<Inscription>> GetByEtudiantIdAsync(int etudiantId);
}
