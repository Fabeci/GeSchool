using FluentValidation;
using GeSchool.Application.DTOs.CoursDtos;
using GeSchool.Application.DTOs.Departements;
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
public class CoursController : Controller
{
    private readonly ICoursService _coursService;
    private readonly IDepartementService _departementService;
    private readonly IEnseignantService _enseignantService;
    private readonly IValidator<CreateCoursDto> _createValidator;
    private readonly IValidator<UpdateCoursDto> _updateValidator;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoursController(
        ICoursService coursService,
        IDepartementService departementService,
        IEnseignantService enseignantService,
        IValidator<CreateCoursDto> createValidator,
        IValidator<UpdateCoursDto> updateValidator,
        UserManager<ApplicationUser> userManager)
    {
        _coursService = coursService;
        _departementService = departementService;
        _enseignantService = enseignantService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, string sortBy = "Code", string sortDir = "asc")
    {
        var cours = await _coursService.GetAllAsync();

        if (IsScopedToOwnEnseignant())
        {
            var enseignantId = await GetCurrentEnseignantIdAsync();
            cours = cours.Where(c => enseignantId.HasValue && c.EnseignantId == enseignantId.Value).ToList();
        }

        var sorted = sortBy switch
        {
            "Intitule" => SortHelper.Apply(cours, c => c.Intitule, sortDir),
            "Credits" => SortHelper.Apply(cours, c => c.Credits, sortDir),
            "Semestre" => SortHelper.Apply(cours, c => c.Semestre, sortDir),
            _ => SortHelper.Apply(cours, c => c.Code, sortDir)
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;
        await PopulateLookupsAsync();
        await PopulateDropdownsAsync();
        return View(PagedList<CoursDto>.Create(sorted, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var cours = await _coursService.GetByIdAsync(id);
        if (cours is null)
        {
            return NotFound();
        }

        if (IsScopedToOwnEnseignant())
        {
            var enseignantId = await GetCurrentEnseignantIdAsync();
            if (enseignantId is null || cours.EnseignantId != enseignantId.Value)
            {
                return Forbid();
            }
        }

        ViewData["BreadcrumbParent"] = ("Cours", Url.Action(nameof(Index)) ?? "/Cours");
        await PopulateLookupsAsync();
        return View(cours);
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCoursDto());
    }

    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCoursDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            await PopulateDropdownsAsync();
            return View(dto);
        }

        try
        {
            await _coursService.CreateAsync(dto);
            TempData["Success"] = "Cours créé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Ce code de cours existe déjà.");
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Edit(int id)
    {
        var cours = await _coursService.GetByIdAsync(id);
        if (cours is null)
        {
            return NotFound();
        }

        var dto = new UpdateCoursDto
        {
            Id = cours.Id,
            Code = cours.Code,
            Intitule = cours.Intitule,
            Credits = cours.Credits,
            Semestre = cours.Semestre,
            DepartementId = cours.DepartementId,
            EnseignantId = cours.EnseignantId
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCoursDto dto)
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

            await PopulateDropdownsAsync();
            return View(dto);
        }

        try
        {
            await _coursService.UpdateAsync(dto);
            TempData["Success"] = "Cours modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Ce code de cours existe déjà.");
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id)
    {
        var cours = await _coursService.GetByIdAsync(id);
        if (cours is null)
        {
            return NotFound();
        }

        return View(cours);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrateur")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _coursService.DeleteAsync(id);
            TempData["Success"] = "Cours supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            var cours = await _coursService.GetByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Impossible de supprimer : ce cours a encore des inscriptions.");
            return View(cours);
        }
    }

    private async Task PopulateDropdownsAsync()
    {
        var departements = await _departementService.GetAllAsync();
        ViewBag.Departements = new SelectList(departements, nameof(DepartementDto.Id), nameof(DepartementDto.Nom));

        var enseignants = await _enseignantService.GetAllAsync();
        var enseignantItems = enseignants.Select(e => new { e.Id, NomComplet = $"{e.Nom} {e.Prenom}" });
        ViewBag.Enseignants = new SelectList(enseignantItems, "Id", "NomComplet");
    }

    private async Task PopulateLookupsAsync()
    {
        ViewBag.DepartementsById = (await _departementService.GetAllAsync()).ToDictionary(d => d.Id);
        ViewBag.EnseignantsById = (await _enseignantService.GetAllAsync()).ToDictionary(e => e.Id);
    }

    private bool IsScopedToOwnEnseignant() => User.IsInRole("Enseignant") && !User.IsInRole("Administrateur");

    private async Task<int?> GetCurrentEnseignantIdAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        return currentUser?.EnseignantId;
    }
}
