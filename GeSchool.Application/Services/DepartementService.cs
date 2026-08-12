using AutoMapper;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class DepartementService : IDepartementService
{
    private readonly IDepartementRepository _repository;
    private readonly IMapper _mapper;

    public DepartementService(IDepartementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartementDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<DepartementDto>(entity);
    }

    public async Task<IReadOnlyList<DepartementDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<DepartementDto>>(entities);
    }

    public async Task<DepartementDto> CreateAsync(CreateDepartementDto dto)
    {
        var entity = _mapper.Map<Departement>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<DepartementDto>(entity);
    }

    public async Task UpdateAsync(UpdateDepartementDto dto)
    {
        var entity = _mapper.Map<Departement>(dto);
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
