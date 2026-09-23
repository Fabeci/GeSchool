using FluentValidation;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.DTOs.Etudiants;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Infrastructure.Identity;
using GeSchool.web.Extensions;
using GeSchool.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur,Enseignant")]
public class EtudiantsController : Controller
{
    private readonly IEtudiantService _etudiantService;
    private readonly IDepartementService _departementService;
    private readonly ICoursService _coursService;
    private readonly IInscriptionService _inscriptionService;
    private readonly IValidator<CreateEtudiantDto> _createValidator;
    private readonly IValidator<UpdateEtudiantDto> _updateValidator;
    private readonly UserManager<ApplicationUser> _userManager;

    public EtudiantsController(
        IEtudiantService etudiantService,
        IDepartementService departementService,
        ICoursService coursService,
        IInscriptionService inscriptionService,
        IValidator<CreateEtudiantDto> createValidator,
        IValidator<UpdateEtudiantDto> updateValidator,
        UserManager<ApplicationUser> userManager)
    {
        _etudiantService = etudiantService;
        _departementService = departementService;
        _coursService = coursService;
        _inscriptionService = inscriptionService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, string sortBy = "Nom", string sortDir = "asc")
    {
        var etudiants = await _etudiantService.GetAllAsync();

        if (IsScopedToOwnEnseignant())
        {
            var ownIds = await GetOwnEtudiantIdsAsync();
            etudiants = etudiants.Where(e => ownIds.Contains(e.Id)).ToList();
        }

        var sorted = sortBy switch
        {
            "Prenom" => SortHelper.Apply(etudiants, e => e.Prenom, sortDir),
            "NumeroEtudiant" => SortHelper.Apply(etudiants, e => e.NumeroEtudiant, sortDir),
            "Email" => SortHelper.Apply(etudiants, e => e.Email, sortDir),
            _ => SortHelper.Apply(etudiants, e => e.Nom, sortDir)
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;
        await PopulateDepartementsAsync();
        return View(PagedList<EtudiantDto>.Create(sorted, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        if (IsScopedToOwnEnseignant())
        {
            var ownIds = await GetOwnEtudiantIdsAsync();
            if (!ownIds.Contains(id))
            {
                return Forbid();
            }
        }

        ViewData["BreadcrumbParent"] = ("Étudiants", Url.Action(nameof(Index)) ?? "/Etudiants");
        return View(etudiant);
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create()
    {
        await PopulateDepartementsAsync();
        return View(new CreateEtudiantDto());
    }

    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEtudiantDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            await PopulateDepartementsAsync();
            return View(dto);
        }

        try
        {
            await _etudiantService.CreateAsync(dto);
            TempData["Success"] = "Étudiant créé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Ce numéro étudiant existe déjà.");
            await PopulateDepartementsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Edit(int id)
    {
        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        var dto = new UpdateEtudiantDto
        {
            Id = etudiant.Id,
            NumeroEtudiant = etudiant.NumeroEtudiant,
            Nom = etudiant.Nom,
            Prenom = etudiant.Prenom,
            Email = etudiant.Email,
            DateNaissance = etudiant.DateNaissance,
            DepartementId = etudiant.DepartementId
        };

        await PopulateDepartementsAsync();
        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateEtudiantDto dto)
    {
        if (id != dto.Id)
        {
            return NotFound();
        }

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            await PopulateDepartementsAsync();
            return View(dto);
        }

        try
        {
            await _etudiantService.UpdateAsync(dto);
            TempData["Success"] = "Étudiant modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Ce numéro étudiant existe déjà.");
            await PopulateDepartementsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id)
    {
        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        return View(etudiant);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _etudiantService.DeleteAsync(id);
            TempData["Success"] = "Étudiant supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            var etudiant = await _etudiantService.GetByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Impossible de supprimer : cet étudiant a encore des inscriptions.");
            return View(etudiant);
        }
    }

    private async Task PopulateDepartementsAsync()
    {
        var departements = await _departementService.GetAllAsync();
        ViewBag.Departements = new SelectList(departements, nameof(DepartementDto.Id), nameof(DepartementDto.Nom));
    }

    private bool IsScopedToOwnEnseignant() => User.IsInRole("Enseignant") && !User.IsInRole("Administrateur");

    private async Task<HashSet<int>> GetOwnEtudiantIdsAsync()
    {
        var enseignantId = (await _userManager.GetUserAsync(User))?.EnseignantId;
        if (enseignantId is null)
        {
            return new HashSet<int>();
        }

        var coursIds = (await _coursService.GetAllAsync())
            .Where(c => c.EnseignantId == enseignantId.Value)
            .Select(c => c.Id)
            .ToHashSet();

        return (await _inscriptionService.GetAllAsync())
            .Where(i => coursIds.Contains(i.CoursId))
            .Select(i => i.EtudiantId)
            .ToHashSet();
    }
}
