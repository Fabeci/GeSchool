using AutoMapper;
using GeSchool.Application.DTOs.CoursDtos;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.DTOs.Enseignants;
using GeSchool.Application.DTOs.Etudiants;
using GeSchool.Application.DTOs.Inscriptions;
using GeSchool.Application.DTOs.Notes;
using GeSchool.Domain.Entities;

namespace GeSchool.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Departement, DepartementDto>().ReverseMap();
        CreateMap<CreateDepartementDto, Departement>();
        CreateMap<UpdateDepartementDto, Departement>();

        CreateMap<Enseignant, EnseignantDto>().ReverseMap();
        CreateMap<CreateEnseignantDto, Enseignant>();
        CreateMap<UpdateEnseignantDto, Enseignant>();

        CreateMap<Etudiant, EtudiantDto>().ReverseMap();
        CreateMap<CreateEtudiantDto, Etudiant>();
        CreateMap<UpdateEtudiantDto, Etudiant>();

        CreateMap<Cours, CoursDto>().ReverseMap();
        CreateMap<CreateCoursDto, Cours>();
        CreateMap<UpdateCoursDto, Cours>();

        CreateMap<Inscription, InscriptionDto>().ReverseMap();
        CreateMap<CreateInscriptionDto, Inscription>();
        CreateMap<UpdateInscriptionDto, Inscription>();

        CreateMap<Note, NoteDto>().ReverseMap();
        CreateMap<CreateNoteDto, Note>();
        CreateMap<UpdateNoteDto, Note>();
    }
}
