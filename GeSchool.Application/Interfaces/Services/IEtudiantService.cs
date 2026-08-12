using GeSchool.Application.DTOs.Etudiants;

namespace GeSchool.Application.Interfaces.Services;

public interface IEtudiantService
{
    Task<EtudiantDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<EtudiantDto>> GetAllAsync();
    Task<EtudiantDto> CreateAsync(CreateEtudiantDto dto);
    Task UpdateAsync(UpdateEtudiantDto dto);
    Task DeleteAsync(int id);
}
