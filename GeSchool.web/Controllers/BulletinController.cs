using GeSchool.Application.Interfaces.Services;
using GeSchool.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeSchool.web.Controllers;

[Authorize]
public class BulletinController : Controller
{
    private readonly IBulletinService _bulletinService;
    private readonly IEtudiantService _etudiantService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BulletinController(IBulletinService bulletinService, IEtudiantService etudiantService, UserManager<ApplicationUser> userManager)
    {
        _bulletinService = bulletinService;
        _etudiantService = etudiantService;
        _userManager = userManager;
    }

    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Index()
    {
        var etudiants = await _etudiantService.GetAllAsync();
        return View(etudiants);
    }

    [Authorize(Roles = "Administrateur,Etudiant")]
    public async Task<IActionResult> Details(int id)
    {
        if (User.IsInRole("Etudiant") && !User.IsInRole("Administrateur"))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.EtudiantId != id)
            {
                return Forbid();
            }
        }

        try
        {
            var bulletin = await _bulletinService.GenerateAsync(id);
            return View(bulletin);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Etudiant")]
    public async Task<IActionResult> Mine()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.EtudiantId is null)
        {
            return View("NotLinked");
        }

        return RedirectToAction(nameof(Details), new { id = currentUser.EtudiantId.Value });
    }
}
