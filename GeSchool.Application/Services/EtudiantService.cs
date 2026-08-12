using AutoMapper;
using GeSchool.Application.DTOs.Etudiants;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class EtudiantService : IEtudiantService
{
    private readonly IEtudiantRepository _repository;
    private readonly IMapper _mapper;

    public EtudiantService(IEtudiantRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EtudiantDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<EtudiantDto>(entity);
    }

    public async Task<IReadOnlyList<EtudiantDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<EtudiantDto>>(entities);
    }

    public async Task<EtudiantDto> CreateAsync(CreateEtudiantDto dto)
    {
        var entity = _mapper.Map<Etudiant>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<EtudiantDto>(entity);
    }

    public async Task UpdateAsync(UpdateEtudiantDto dto)
    {
        var entity = _mapper.Map<Etudiant>(dto);
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
