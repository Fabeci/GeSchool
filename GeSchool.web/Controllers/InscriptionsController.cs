using FluentValidation;
using GeSchool.Application.DTOs.CoursDtos;
using GeSchool.Application.DTOs.Etudiants;
using GeSchool.Application.DTOs.Inscriptions;
using GeSchool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class InscriptionsController : Controller
{
    private readonly IInscriptionService _inscriptionService;
    private readonly IEtudiantService _etudiantService;
    private readonly ICoursService _coursService;
    private readonly IValidator<CreateInscriptionDto> _createValidator;
    private readonly IValidator<UpdateInscriptionDto> _updateValidator;

    public InscriptionsController(
        IInscriptionService inscriptionService,
        IEtudiantService etudiantService,
        ICoursService coursService,
        IValidator<CreateInscriptionDto> createValidator,
        IValidator<UpdateInscriptionDto> updateValidator)
    {
        _inscriptionService = inscriptionService;
        _etudiantService = etudiantService;
        _coursService = coursService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var inscriptions = await _inscriptionService.GetAllAsync();
        await PopulateLookupsAsync();
        return View(inscriptions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var inscription = await _inscriptionService.GetByIdAsync(id);
        if (inscription is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync();
        return View(inscription);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new CreateInscriptionDto { DateInscription = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInscriptionDto dto)
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
            await _inscriptionService.CreateAsync(dto);
            TempData["Success"] = "Inscription créée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Cet étudiant est déjà inscrit à ce cours.");
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var inscription = await _inscriptionService.GetByIdAsync(id);
        if (inscription is null)
        {
            return NotFound();
        }

        var dto = new UpdateInscriptionDto
        {
            Id = inscription.Id,
            EtudiantId = inscription.EtudiantId,
            CoursId = inscription.CoursId,
            DateInscription = inscription.DateInscription
        };

        await PopulateDropdownsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateInscriptionDto dto)
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
            await _inscriptionService.UpdateAsync(dto);
            TempData["Success"] = "Inscription modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Cet étudiant est déjà inscrit à ce cours.");
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var inscription = await _inscriptionService.GetByIdAsync(id);
        if (inscription is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync();
        return View(inscription);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _inscriptionService.DeleteAsync(id);
        TempData["Success"] = "Inscription supprimée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var etudiants = await _etudiantService.GetAllAsync();
        var etudiantItems = etudiants.Select(e => new { e.Id, Label = $"{e.NumeroEtudiant} - {e.Nom} {e.Prenom}" });
        ViewBag.EtudiantOptions = new SelectList(etudiantItems, "Id", "Label");

        var cours = await _coursService.GetAllAsync();
        var coursItems = cours.Select(c => new { c.Id, Label = $"{c.Code} - {c.Intitule}" });
        ViewBag.CoursOptions = new SelectList(coursItems, "Id", "Label");
    }

    private async Task PopulateLookupsAsync()
    {
        var etudiants = await _etudiantService.GetAllAsync();
        ViewBag.Etudiants = etudiants.ToDictionary(e => e.Id);

        var cours = await _coursService.GetAllAsync();
        ViewBag.Cours = cours.ToDictionary(c => c.Id);
    }
}
