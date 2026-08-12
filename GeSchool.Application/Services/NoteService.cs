using AutoMapper;
using GeSchool.Application.DTOs.Notes;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _repository;
    private readonly IMapper _mapper;

    public NoteService(INoteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<NoteDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<NoteDto>(entity);
    }

    public async Task<IReadOnlyList<NoteDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<NoteDto>>(entities);
    }

    public async Task<NoteDto> CreateAsync(CreateNoteDto dto)
    {
        var entity = _mapper.Map<Note>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<NoteDto>(entity);
    }

    public async Task UpdateAsync(UpdateNoteDto dto)
    {
        var entity = _mapper.Map<Note>(dto);
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return;
        }

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}
