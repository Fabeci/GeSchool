using FluentValidation;
using GeSchool.Application.DTOs.Notes;
using GeSchool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur")]
public class NotesController : Controller
{
    private readonly INoteService _noteService;
    private readonly IInscriptionService _inscriptionService;
    private readonly IEtudiantService _etudiantService;
    private readonly ICoursService _coursService;
    private readonly IValidator<CreateNoteDto> _createValidator;
    private readonly IValidator<UpdateNoteDto> _updateValidator;

    public NotesController(
        INoteService noteService,
        IInscriptionService inscriptionService,
        IEtudiantService etudiantService,
        ICoursService coursService,
        IValidator<CreateNoteDto> createValidator,
        IValidator<UpdateNoteDto> updateValidator)
    {
        _noteService = noteService;
        _inscriptionService = inscriptionService;
        _etudiantService = etudiantService;
        _coursService = coursService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IActionResult> Index()
    {
        var notes = await _noteService.GetAllAsync();
        ViewBag.InscriptionLabels = await BuildInscriptionLabelsAsync();
        return View(notes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        ViewBag.InscriptionLabels = await BuildInscriptionLabelsAsync();
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateInscriptionsAsync();
        return View(new CreateNoteDto { DateEvaluation = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNoteDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            await PopulateInscriptionsAsync();
            return View(dto);
        }

        await _noteService.CreateAsync(dto);
        TempData["Success"] = "Note créée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        var dto = new UpdateNoteDto
        {
            Id = note.Id,
            InscriptionId = note.InscriptionId,
            Valeur = note.Valeur,
            TypeEvaluation = note.TypeEvaluation,
            DateEvaluation = note.DateEvaluation
        };

        await PopulateInscriptionsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateNoteDto dto)
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

            await PopulateInscriptionsAsync();
            return View(dto);
        }

        await _noteService.UpdateAsync(dto);
        TempData["Success"] = "Note modifiée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        ViewBag.InscriptionLabels = await BuildInscriptionLabelsAsync();
        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _noteService.DeleteAsync(id);
        TempData["Success"] = "Note supprimée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateInscriptionsAsync()
    {
        var labels = await BuildInscriptionLabelsAsync();
        var items = labels.Select(kvp => new { Id = kvp.Key, Label = kvp.Value });
        ViewBag.InscriptionOptions = new SelectList(items, "Id", "Label");
    }

    private async Task<Dictionary<int, string>> BuildInscriptionLabelsAsync()
    {
        var inscriptions = await _inscriptionService.GetAllAsync();
        var etudiants = (await _etudiantService.GetAllAsync()).ToDictionary(e => e.Id);
        var cours = (await _coursService.GetAllAsync()).ToDictionary(c => c.Id);

        return inscriptions.ToDictionary(
            i => i.Id,
            i =>
            {
                var etudiantLabel = etudiants.TryGetValue(i.EtudiantId, out var e) ? e.NumeroEtudiant : i.EtudiantId.ToString();
                var coursLabel = cours.TryGetValue(i.CoursId, out var c) ? c.Code : i.CoursId.ToString();
                return $"{etudiantLabel} - {coursLabel}";
            });
    }
}
