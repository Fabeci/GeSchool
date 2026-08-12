using GeSchool.Application.DTOs.Departements;

namespace GeSchool.Application.Interfaces.Services;

public interface IDepartementService
{
    Task<DepartementDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<DepartementDto>> GetAllAsync();
    Task<DepartementDto> CreateAsync(CreateDepartementDto dto);
    Task UpdateAsync(UpdateDepartementDto dto);
    Task DeleteAsync(int id);
}
