using FluentValidation;
using GeSchool.Application.DTOs.CoursDtos;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class CoursController : Controller
{
    private readonly ICoursService _coursService;
    private readonly IDepartementService _departementService;
    private readonly IEnseignantService _enseignantService;
    private readonly IValidator<CreateCoursDto> _createValidator;
    private readonly IValidator<UpdateCoursDto> _updateValidator;

    public CoursController(
        ICoursService coursService,
        IDepartementService departementService,
        IEnseignantService enseignantService,
        IValidator<CreateCoursDto> createValidator,
        IValidator<UpdateCoursDto> updateValidator)
    {
        _coursService = coursService;
        _departementService = departementService;
        _enseignantService = enseignantService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var cours = await _coursService.GetAllAsync();
        await PopulateLookupsAsync();
        return View(cours);
    }

    public async Task<IActionResult> Details(int id)
    {
        var cours = await _coursService.GetByIdAsync(id);
        if (cours is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync();
        return View(cours);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateCoursDto());
    }

    [HttpPost]
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
}
