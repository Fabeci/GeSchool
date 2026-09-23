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

[Authorize(Roles = "Administrateur")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEtudiantService _etudiantService;
    private readonly IEnseignantService _enseignantService;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEtudiantService etudiantService,
        IEnseignantService enseignantService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _etudiantService = etudiantService;
        _enseignantService = enseignantService;
    }

    public async Task<IActionResult> Index(int page = 1, string sortBy = "Email", string sortDir = "asc")
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

        var items = new List<UserListItemViewModel>();
        foreach (var user in users)
        {
            items.Add(await ToViewModelAsync(user));
        }

        var sorted = sortBy switch
        {
            "Nom" => SortHelper.Apply(items, i => i.Nom, sortDir),
            _ => SortHelper.Apply(items, i => i.Email, sortDir)
        };

        var lookupModel = new CreateUserViewModel { AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList() };
        await PopulateEtudiantsAsync(lookupModel);
        await PopulateEnseignantsAsync(lookupModel);
        ViewBag.AvailableRoles = lookupModel.AvailableRoles;
        ViewBag.AvailableEtudiants = lookupModel.AvailableEtudiants;
        ViewBag.AvailableEnseignants = lookupModel.AvailableEnseignants;
        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;

        return View(PagedList<UserListItemViewModel>.Create(sorted, page));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateUserViewModel { AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList() };
        await PopulateEtudiantsAsync(model);
        await PopulateEnseignantsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
            await PopulateEtudiantsAsync(model);
            await PopulateEnseignantsAsync(model);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            Nom = model.Nom,
            Prenom = model.Prenom,
            EtudiantId = model.EtudiantId,
            EnseignantId = model.EnseignantId
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role);
            TempData["Success"] = "Utilisateur créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        model.AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
        await PopulateEtudiantsAsync(model);
        await PopulateEnseignantsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return View(await ToViewModelAsync(user));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        if (user.Id == _userManager.GetUserId(User))
        {
            ModelState.AddModelError(string.Empty, "Vous ne pouvez pas supprimer votre propre compte.");
            return View(await ToViewModelAsync(user));
        }

        if (await _userManager.IsInRoleAsync(user, "Administrateur"))
        {
            var admins = await _userManager.GetUsersInRoleAsync("Administrateur");
            if (admins.Count <= 1)
            {
                ModelState.AddModelError(string.Empty, "Impossible de supprimer le dernier compte Administrateur.");
                return View(await ToViewModelAsync(user));
            }
        }

        await _userManager.DeleteAsync(user);
        TempData["Success"] = "Utilisateur supprimé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateEtudiantsAsync(CreateUserViewModel model)
    {
        var etudiants = await _etudiantService.GetAllAsync();
        model.AvailableEtudiants = etudiants
            .Select(e => new SelectListItem($"{e.NumeroEtudiant} - {e.Nom} {e.Prenom}", e.Id.ToString()))
            .ToList();
    }

    private async Task PopulateEnseignantsAsync(CreateUserViewModel model)
    {
        var enseignants = await _enseignantService.GetAllAsync();
        model.AvailableEnseignants = enseignants
            .Select(e => new SelectListItem($"{e.Nom} {e.Prenom}", e.Id.ToString()))
            .ToList();
    }

    private async Task<UserListItemViewModel> ToViewModelAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserListItemViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Nom = user.Nom,
            Prenom = user.Prenom,
            Roles = roles.ToList()
        };
    }
}
