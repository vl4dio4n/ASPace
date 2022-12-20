using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASPace.Models;
using ASPace.Areas.Identity.Data;
using ASPace.Data;
using Microsoft.AspNetCore.Identity;

namespace ASPace.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        UserManager<ApplicationUser> userManager
    )
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        ViewBag.user = _userManager.GetUserName(User);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}