using GeSchool.Application.Interfaces.Services;
using GeSchool.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeSchool.web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IEtudiantService _etudiantService;
    private readonly IEnseignantService _enseignantService;
    private readonly ICoursService _coursService;
    private readonly IInscriptionService _inscriptionService;

    public DashboardController(
        IEtudiantService etudiantService,
        IEnseignantService enseignantService,
        ICoursService coursService,
        IInscriptionService inscriptionService)
    {
        _etudiantService = etudiantService;
        _enseignantService = enseignantService;
        _coursService = coursService;
        _inscriptionService = inscriptionService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            EtudiantsCount = (await _etudiantService.GetAllAsync()).Count,
            EnseignantsCount = (await _enseignantService.GetAllAsync()).Count,
            CoursCount = (await _coursService.GetAllAsync()).Count,
            InscriptionsCount = (await _inscriptionService.GetAllAsync()).Count
        };

        return View(model);
    }
}
