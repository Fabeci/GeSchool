using GeSchool.Application.DTOs.Enseignants;

namespace GeSchool.Application.Interfaces.Services;

public interface IEnseignantService
{
    Task<EnseignantDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<EnseignantDto>> GetAllAsync();
    Task<EnseignantDto> CreateAsync(CreateEnseignantDto dto);
    Task UpdateAsync(UpdateEnseignantDto dto);
    Task DeleteAsync(int id);
}
