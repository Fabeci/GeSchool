using FluentValidation;
using GeSchool.Application.DTOs.Departements;
using GeSchool.Application.DTOs.Etudiants;
using GeSchool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class EtudiantsController : Controller
{
    private readonly IEtudiantService _etudiantService;
    private readonly IDepartementService _departementService;
    private readonly IValidator<CreateEtudiantDto> _createValidator;
    private readonly IValidator<UpdateEtudiantDto> _updateValidator;

    public EtudiantsController(
        IEtudiantService etudiantService,
        IDepartementService departementService,
        IValidator<CreateEtudiantDto> createValidator,
        IValidator<UpdateEtudiantDto> updateValidator)
    {
        _etudiantService = etudiantService;
        _departementService = departementService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var etudiants = await _etudiantService.GetAllAsync();
        return View(etudiants);
    }

    public async Task<IActionResult> Details(int id)
    {
        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        return View(etudiant);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDepartementsAsync();
        return View(new CreateEtudiantDto());
    }

    [HttpPost]
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
}
