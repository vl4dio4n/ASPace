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
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        db = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public ActionResult Index()
    {
        ViewBag.user = _userManager.GetUserName(User);
        string CurrentUserId = _userManager.GetUserId(User);
        ViewBag.CurrentUser = db.Users.Find(CurrentUserId);
        return View();
    }

    public ActionResult About()
    {
        ViewBag.Message = "Your application description page.";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

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