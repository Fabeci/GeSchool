using GeSchool.Application.DTOs.Notes;

namespace GeSchool.Application.Interfaces.Services;

public interface INoteService
{
    Task<NoteDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<NoteDto>> GetAllAsync();
    Task<NoteDto> CreateAsync(CreateNoteDto dto);
    Task UpdateAsync(UpdateNoteDto dto);
    Task DeleteAsync(int id);
}
