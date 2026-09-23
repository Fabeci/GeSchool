using FluentValidation;
using GeSchool.Application.DTOs.Notes;
using GeSchool.Application.Interfaces.Services;
using GeSchool.Infrastructure.Identity;
using GeSchool.web.Extensions;
using GeSchool.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeSchool.web.Controllers;

[Authorize(Roles = "Administrateur,Enseignant")]
public class NotesController : Controller
{
    private readonly INoteService _noteService;
    private readonly IInscriptionService _inscriptionService;
    private readonly IEtudiantService _etudiantService;
    private readonly ICoursService _coursService;
    private readonly IValidator<CreateNoteDto> _createValidator;
    private readonly IValidator<UpdateNoteDto> _updateValidator;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotesController(
        INoteService noteService,
        IInscriptionService inscriptionService,
        IEtudiantService etudiantService,
        ICoursService coursService,
        IValidator<CreateNoteDto> createValidator,
        IValidator<UpdateNoteDto> updateValidator,
        UserManager<ApplicationUser> userManager)
    {
        _noteService = noteService;
        _inscriptionService = inscriptionService;
        _etudiantService = etudiantService;
        _coursService = coursService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, string sortBy = "DateEvaluation", string sortDir = "desc")
    {
        var notes = await _noteService.GetAllAsync();

        if (IsScopedToOwnEnseignant())
        {
            var ownInscriptionIds = await GetOwnInscriptionIdsAsync();
            notes = notes.Where(n => ownInscriptionIds.Contains(n.InscriptionId)).ToList();
        }

        var sorted = sortBy switch
        {
            "Valeur" => SortHelper.Apply(notes, n => n.Valeur, sortDir),
            _ => SortHelper.Apply(notes, n => n.DateEvaluation, sortDir)
        };

        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;
        ViewBag.InscriptionLabels = await BuildInscriptionLabelsAsync();
        await PopulateInscriptionsAsync();
        return View(PagedList<NoteDto>.Create(sorted, page));
    }

    public async Task<IActionResult> Details(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        if (IsScopedToOwnEnseignant() && !(await GetOwnInscriptionIdsAsync()).Contains(note.InscriptionId))
        {
            return Forbid();
        }

        ViewData["BreadcrumbParent"] = ("Notes", Url.Action(nameof(Index)) ?? "/Notes");
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

        if (IsScopedToOwnEnseignant() && !(await GetOwnInscriptionIdsAsync()).Contains(dto.InscriptionId))
        {
            return Forbid();
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

        if (IsScopedToOwnEnseignant() && !(await GetOwnInscriptionIdsAsync()).Contains(note.InscriptionId))
        {
            return Forbid();
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

        var existing = await _noteService.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (IsScopedToOwnEnseignant())
        {
            var ownInscriptionIds = await GetOwnInscriptionIdsAsync();
            if (!ownInscriptionIds.Contains(existing.InscriptionId) || !ownInscriptionIds.Contains(dto.InscriptionId))
            {
                return Forbid();
            }
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

        if (IsScopedToOwnEnseignant() && !(await GetOwnInscriptionIdsAsync()).Contains(note.InscriptionId))
        {
            return Forbid();
        }

        ViewBag.InscriptionLabels = await BuildInscriptionLabelsAsync();
        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }

        if (IsScopedToOwnEnseignant() && !(await GetOwnInscriptionIdsAsync()).Contains(note.InscriptionId))
        {
            return Forbid();
        }

        await _noteService.DeleteAsync(id);
        TempData["Success"] = "Note supprimée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateInscriptionsAsync()
    {
        var labels = await BuildInscriptionLabelsAsync();
        var items = labels.Select(kvp => new { Id = kvp.Key, Label = kvp.Value });

        if (IsScopedToOwnEnseignant())
        {
            var ownInscriptionIds = await GetOwnInscriptionIdsAsync();
            items = items.Where(i => ownInscriptionIds.Contains(i.Id));
        }

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

    private bool IsScopedToOwnEnseignant() => User.IsInRole("Enseignant") && !User.IsInRole("Administrateur");

    private async Task<HashSet<int>> GetOwnInscriptionIdsAsync()
    {
        var enseignantId = (await _userManager.GetUserAsync(User))?.EnseignantId;
        if (enseignantId is null)
        {
            return new HashSet<int>();
        }

        var ownCoursIds = (await _coursService.GetAllAsync())
            .Where(c => c.EnseignantId == enseignantId.Value)
            .Select(c => c.Id)
            .ToHashSet();

        return (await _inscriptionService.GetAllAsync())
            .Where(i => ownCoursIds.Contains(i.CoursId))
            .Select(i => i.Id)
            .ToHashSet();
    }
}
