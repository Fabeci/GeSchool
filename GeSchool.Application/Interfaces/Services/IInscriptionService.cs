using GeSchool.Application.DTOs.Inscriptions;

namespace GeSchool.Application.Interfaces.Services;

public interface IInscriptionService
{
    Task<InscriptionDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<InscriptionDto>> GetAllAsync();
    Task<InscriptionDto> CreateAsync(CreateInscriptionDto dto);
    Task UpdateAsync(UpdateInscriptionDto dto);
    Task DeleteAsync(int id);
}
