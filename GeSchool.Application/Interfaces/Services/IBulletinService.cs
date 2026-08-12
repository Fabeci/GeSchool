using GeSchool.Application.DTOs.Notes;

namespace GeSchool.Application.Interfaces.Services;

public interface IBulletinService
{
    Task<BulletinDto> GenerateAsync(int etudiantId);
}
