using AutoMapper;
using GeSchool.Application.DTOs.CoursDtos;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Services;

public class CoursService : ICoursService
{
    private readonly ICoursRepository _repository;
    private readonly IMapper _mapper;

    public CoursService(ICoursRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CoursDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<CoursDto>(entity);
    }

    public async Task<IReadOnlyList<CoursDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<CoursDto>>(entities);
    }

    public async Task<CoursDto> CreateAsync(CreateCoursDto dto)
    {
        var entity = _mapper.Map<Cours>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<CoursDto>(entity);
    }

    public async Task UpdateAsync(UpdateCoursDto dto)
    {
        var entity = _mapper.Map<Cours>(dto);
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
