using GeSchool.Application.Interfaces.Services;
using GeSchool.Infrastructure.Identity;
using GeSchool.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Etudiant")]
public class ProfilController : Controller
{
    private readonly IEtudiantService _etudiantService;
    private readonly IInscriptionService _inscriptionService;
    private readonly ICoursService _coursService;
    private readonly IEnseignantService _enseignantService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfilController(
        IEtudiantService etudiantService,
        IInscriptionService inscriptionService,
        ICoursService coursService,
        IEnseignantService enseignantService,
        UserManager<ApplicationUser> userManager)
    {
        _etudiantService = etudiantService;
        _inscriptionService = inscriptionService;
        _coursService = coursService;
        _enseignantService = enseignantService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var etudiantId = await GetCurrentEtudiantIdAsync();
        if (etudiantId is null)
        {
            return NotLinkedView();
        }

        var etudiant = await _etudiantService.GetByIdAsync(etudiantId.Value);
        if (etudiant is null)
        {
            return NotLinkedView();
        }

        return View(etudiant);
    }

    public async Task<IActionResult> MesCours()
    {
        var etudiantId = await GetCurrentEtudiantIdAsync();
        if (etudiantId is null)
        {
            return NotLinkedView();
        }

        var inscriptions = (await _inscriptionService.GetAllAsync())
            .Where(i => i.EtudiantId == etudiantId.Value)
            .ToList();

        var coursById = (await _coursService.GetAllAsync()).ToDictionary(c => c.Id);
        var enseignantsById = (await _enseignantService.GetAllAsync()).ToDictionary(e => e.Id);

        var items = new List<MonCoursViewModel>();
        foreach (var inscription in inscriptions)
        {
            if (!coursById.TryGetValue(inscription.CoursId, out var cours))
            {
                continue;
            }

            var enseignantNom = enseignantsById.TryGetValue(cours.EnseignantId, out var enseignant)
                ? $"{enseignant.Nom} {enseignant.Prenom}"
                : "—";

            items.Add(new MonCoursViewModel
            {
                Code = cours.Code,
                Intitule = cours.Intitule,
                Credits = cours.Credits,
                Semestre = cours.Semestre,
                EnseignantNom = enseignantNom,
                DateInscription = inscription.DateInscription
            });
        }

        return View(items);
    }

    private IActionResult NotLinkedView()
    {
        return View("NotLinked", new EmptyStateViewModel
        {
            Icon = "info",
            Title = "Compte non lié",
            Message = "Votre compte n'est pas encore lié à une fiche étudiant. Contactez l'administration pour faire le lien."
        });
    }

    private async Task<int?> GetCurrentEtudiantIdAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        return currentUser?.EtudiantId;
    }
}
