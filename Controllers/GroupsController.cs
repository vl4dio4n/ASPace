using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPace.Data;
using ASPace.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPace.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        public GroupsController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
