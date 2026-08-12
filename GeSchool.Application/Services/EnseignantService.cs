using AutoMapper;
using GeSchool.Application.DTOs.Enseignants;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class EnseignantService : IEnseignantService
{
    private readonly IEnseignantRepository _repository;
    private readonly IMapper _mapper;

    public EnseignantService(IEnseignantRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EnseignantDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<EnseignantDto>(entity);
    }

    public async Task<IReadOnlyList<EnseignantDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<EnseignantDto>>(entities);
    }

    public async Task<EnseignantDto> CreateAsync(CreateEnseignantDto dto)
    {
        var entity = _mapper.Map<Enseignant>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<EnseignantDto>(entity);
    }

    public async Task UpdateAsync(UpdateEnseignantDto dto)
    {
        var entity = _mapper.Map<Enseignant>(dto);
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
