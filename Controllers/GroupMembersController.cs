using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ASPace.Areas.Identity.Data;
using ASPace.Data;
using ASPace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPace.Controllers
{
    public class GroupMembersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupMembersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [NonAction]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(int id)
        {
            var group = db.Groups.Where(m => m.Id == id).Include("Creator").Include("GroupMembers").First();
            ViewBag.CurrentUser = new Tuple<string, bool>(_userManager.GetUserId(User), group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"));

            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New([FromForm] GroupMember groupmember)
        {
            try
            {
                db.GroupMembers.Add(groupmember);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + groupmember.GroupId);
            }
            catch (Exception e)
            {
                return Redirect("/Groups/Show/" + groupmember.GroupId);
            }

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(int GroupId, string UserId)
        {
            GroupMember ToDelete = db.GroupMembers.Find(UserId, GroupId);
            if (ToDelete != null)
            {
                db.GroupMembers.Remove(ToDelete);
                db.SaveChanges();
                string controller = (string)TempData["ReturnTo"];
            }
            return RedirectToAction("Status", "Groups", new { id = GroupId });
        }
    }
}
