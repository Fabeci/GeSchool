using AutoMapper;
using GeSchool.Application.DTOs.Notes;
using GeSchool.Application.Interfaces.Repositories;
using GeSchool.Application.Interfaces.Services;

namespace GeSchool.Application.Services;

public class BulletinService : IBulletinService
{
    private readonly IEtudiantRepository _etudiantRepository;
    private readonly IInscriptionRepository _inscriptionRepository;
    private readonly ICoursRepository _coursRepository;
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public BulletinService(
        IEtudiantRepository etudiantRepository,
        IInscriptionRepository inscriptionRepository,
        ICoursRepository coursRepository,
        INoteRepository noteRepository,
        IMapper mapper)
    {
        _etudiantRepository = etudiantRepository;
        _inscriptionRepository = inscriptionRepository;
        _coursRepository = coursRepository;
        _noteRepository = noteRepository;
        _mapper = mapper;
    }

    public async Task<BulletinDto> GenerateAsync(int etudiantId)
    {
        var etudiant = await _etudiantRepository.GetByIdAsync(etudiantId)
            ?? throw new KeyNotFoundException($"Aucun étudiant trouvé avec l'id {etudiantId}.");

        var inscriptions = await _inscriptionRepository.GetByEtudiantIdAsync(etudiantId);
        var coursParId = (await _coursRepository.GetAllAsync()).ToDictionary(c => c.Id);

        var bulletinCours = new List<BulletinCoursDto>();
        foreach (var inscription in inscriptions)
        {
            if (!coursParId.TryGetValue(inscription.CoursId, out var cours))
            {
                continue;
            }

            var notes = await _noteRepository.GetByInscriptionIdAsync(inscription.Id);
            var moyenneCours = notes.Count > 0 ? notes.Average(n => n.Valeur) : (decimal?)null;

            bulletinCours.Add(new BulletinCoursDto
            {
                CoursId = cours.Id,
                Code = cours.Code,
                Intitule = cours.Intitule,
                Credits = cours.Credits,
                Notes = _mapper.Map<IReadOnlyList<NoteDto>>(notes),
                MoyenneCours = moyenneCours
            });
        }

        var coursNotes = bulletinCours.Where(c => c.MoyenneCours.HasValue).ToList();
        var sommeCredits = coursNotes.Sum(c => c.Credits);
        var moyenneGenerale = sommeCredits > 0
            ? coursNotes.Sum(c => c.MoyenneCours!.Value * c.Credits) / sommeCredits
            : 0m;

        return new BulletinDto
        {
            EtudiantId = etudiant.Id,
            NumeroEtudiant = etudiant.NumeroEtudiant,
            NomComplet = $"{etudiant.Nom} {etudiant.Prenom}",
            Cours = bulletinCours,
            MoyenneGenerale = moyenneGenerale
        };
    }
}
