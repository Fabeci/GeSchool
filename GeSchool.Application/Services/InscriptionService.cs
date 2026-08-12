using AutoMapper;
using GeSchool.Application.DTOs.Inscriptions;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class InscriptionService : IInscriptionService
{
    private readonly IInscriptionRepository _repository;
    private readonly IMapper _mapper;

    public InscriptionService(IInscriptionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InscriptionDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<InscriptionDto>(entity);
    }

    public async Task<IReadOnlyList<InscriptionDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<InscriptionDto>>(entities);
    }

    public async Task<InscriptionDto> CreateAsync(CreateInscriptionDto dto)
    {
        var entity = _mapper.Map<Inscription>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<InscriptionDto>(entity);
    }

    public async Task UpdateAsync(UpdateInscriptionDto dto)
    {
        var entity = _mapper.Map<Inscription>(dto);
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
