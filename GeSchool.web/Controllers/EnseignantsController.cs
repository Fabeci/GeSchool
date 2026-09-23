using FluentValidation;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.DTOs.Enseignants;
using GeSchool.Application.Interfaces.Services;
using GeSchool.web.Extensions;
using GeSchool.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class EnseignantsController : Controller
{
    private readonly IEnseignantService _enseignantService;
    private readonly IDepartementService _departementService;
    private readonly IValidator<CreateEnseignantDto> _createValidator;
    private readonly IValidator<UpdateEnseignantDto> _updateValidator;

    public EnseignantsController(
        IEnseignantService enseignantService,
        IDepartementService departementService,
        IValidator<CreateEnseignantDto> createValidator,
        IValidator<UpdateEnseignantDto> updateValidator)
    {
        _enseignantService = enseignantService;
        _departementService = departementService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index(int page = 1, string sortBy = "Nom", string sortDir = "asc")
    {
        var enseignants = await _enseignantService.GetAllAsync();

        var sorted = sortBy switch
        {
            "Prenom" => SortHelper.Apply(enseignants, e => e.Prenom, sortDir),
            "Email" => SortHelper.Apply(enseignants, e => e.Email, sortDir),
            "Specialite" => SortHelper.Apply(enseignants, e => e.Specialite, sortDir),
            _ => SortHelper.Apply(enseignants, e => e.Nom, sortDir)
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;
        await PopulateDepartementsAsync();
        return View(PagedList<EnseignantDto>.Create(sorted, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var enseignant = await _enseignantService.GetByIdAsync(id);
        if (enseignant is null)
        {
            return NotFound();
        }

        ViewData["BreadcrumbParent"] = ("Enseignants", Url.Action(nameof(Index)) ?? "/Enseignants");
        return View(enseignant);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDepartementsAsync();
        return View(new CreateEnseignantDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEnseignantDto dto)
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

        await _enseignantService.CreateAsync(dto);
        TempData["Success"] = "Enseignant créé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var enseignant = await _enseignantService.GetByIdAsync(id);
        if (enseignant is null)
        {
            return NotFound();
        }

        var dto = new UpdateEnseignantDto
        {
            Id = enseignant.Id,
            Nom = enseignant.Nom,
            Prenom = enseignant.Prenom,
            Email = enseignant.Email,
            Specialite = enseignant.Specialite,
            DepartementId = enseignant.DepartementId
        };

        await PopulateDepartementsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateEnseignantDto dto)
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

        await _enseignantService.UpdateAsync(dto);
        TempData["Success"] = "Enseignant modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var enseignant = await _enseignantService.GetByIdAsync(id);
        if (enseignant is null)
        {
            return NotFound();
        }

        return View(enseignant);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _enseignantService.DeleteAsync(id);
            TempData["Success"] = "Enseignant supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            var enseignant = await _enseignantService.GetByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Impossible de supprimer : cet enseignant est encore responsable d'au moins un cours.");
            return View(enseignant);
        }
    }

    private async Task PopulateDepartementsAsync()
    {
        var departements = await _departementService.GetAllAsync();
        ViewBag.Departements = new SelectList(departements, nameof(DepartementDto.Id), nameof(DepartementDto.Nom));
    }
}
