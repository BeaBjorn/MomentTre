using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MomentTre.Models;

namespace MomentTre.Controllers;

// HomeController class
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //Returning the Index view
    public IActionResult Index()
    {
        return View();
    }

    // Returning the Privacy view
    public IActionResult Privacy()
    {
        return View();
    }

    // Returning Error view
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
