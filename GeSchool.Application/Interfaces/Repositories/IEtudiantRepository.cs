using GeSchool.Domain.Entities;

namespace GeSchool.Application.Interfaces.Repositories;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    Task<Etudiant?> GetByNumeroEtudiantAsync(string numeroEtudiant);
}
