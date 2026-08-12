using GeSchool.Application.Interfaces.Services;
using GeSchool.Infrastructure.Identity;
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

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEtudiantService etudiantService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _etudiantService = etudiantService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

        var items = new List<UserListItemViewModel>();
        foreach (var user in users)
        {
            items.Add(await ToViewModelAsync(user));
        }

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateUserViewModel { AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList() };
        await PopulateEtudiantsAsync(model);
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
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            Nom = model.Nom,
            Prenom = model.Prenom,
            EtudiantId = model.EtudiantId
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
