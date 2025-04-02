using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort.DbContext;
using Resort.Models;
using Resort.Models.ViewModels;

namespace Resort.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var villaList = _context.Villas.Include(x => x.VillaAmenity).ToList();
        HomeVM homeVM = new HomeVM
        {
            VillaList = villaList,
            Nights = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            CheckOutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };
        return View(homeVM);
    }

    [HttpPost]
    public IActionResult Index(HomeVM homeVM)
    {
        homeVM.VillaList = _context.Villas.Include(x => x.VillaAmenity).ToList();
        foreach (var villa in homeVM.VillaList)
        {
            if (villa.Id % 2 == 0)
            {
                villa.IsAvailable = false;
            }
            else
            {
                villa.IsAvailable = true;
            }
        }
        return View(homeVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
