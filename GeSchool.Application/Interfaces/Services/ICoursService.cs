using GeSchool.Application.DTOs.CoursDtos;

namespace GeSchool.Application.Interfaces.Services;

public interface ICoursService
{
    Task<CoursDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<CoursDto>> GetAllAsync();
    Task<CoursDto> CreateAsync(CreateCoursDto dto);
    Task UpdateAsync(UpdateCoursDto dto);
    Task DeleteAsync(int id);
}
