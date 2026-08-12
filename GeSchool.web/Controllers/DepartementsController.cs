using FluentValidation;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class DepartementsController : Controller
{
    private readonly IDepartementService _departementService;
    private readonly IValidator<CreateDepartementDto> _createValidator;
    private readonly IValidator<UpdateDepartementDto> _updateValidator;

    public DepartementsController(
        IDepartementService departementService,
        IValidator<CreateDepartementDto> createValidator,
        IValidator<UpdateDepartementDto> updateValidator)
    {
        _departementService = departementService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var departements = await _departementService.GetAllAsync();
        return View(departements);
    }

    public async Task<IActionResult> Details(int id)
    {
        var departement = await _departementService.GetByIdAsync(id);
        if (departement is null)
        {
            return NotFound();
        }

        return View(departement);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDepartementDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDepartementDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(dto);
        }

        await _departementService.CreateAsync(dto);
        TempData["Success"] = "Département créé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var departement = await _departementService.GetByIdAsync(id);
        if (departement is null)
        {
            return NotFound();
        }

        var dto = new UpdateDepartementDto
        {
            Id = departement.Id,
            Nom = departement.Nom,
            Description = departement.Description
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateDepartementDto dto)
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

            return View(dto);
        }

        await _departementService.UpdateAsync(dto);
        TempData["Success"] = "Département modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var departement = await _departementService.GetByIdAsync(id);
        if (departement is null)
        {
            return NotFound();
        }

        return View(departement);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _departementService.DeleteAsync(id);
            TempData["Success"] = "Département supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            var departement = await _departementService.GetByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Impossible de supprimer : ce département est encore utilisé par des enseignants, étudiants ou cours.");
            return View(departement);
        }
    }
}
