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

namespace ASPace.Controllers
{
    public class GroupRequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupRequestsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: GroupRequests
        [NonAction]
        public IActionResult Index()
        {
            return View();
        }

        /*  public IActionResult Show(int GroupId)
            {
                var group = db.Groups.Find(GroupId);
                var grouprequests = group.GroupRequests;
                ViewBag.Group = group;
                ViewBag.GroupRequests = grouprequests;
                ViewBag.CurrentUser = new Tuple<string, bool>(User.Identity.GetUserId(), group.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"));
                return View(group);
            }
    */

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New([FromForm] GroupRequest grouprequest)
        {
            try
            {
                db.GroupRequests.Add(grouprequest);
                db.SaveChanges();
                return RedirectToAction("Show", "Groups", new { id = grouprequest.ReceiverId });
            }
            catch (Exception e)
            {
                return RedirectToAction("Show", "Groups", new { id = grouprequest.ReceiverId });
            }

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(string SenderId, int GroupId)
        {
            GroupRequest ToDelete = db.GroupRequests.Find(SenderId, GroupId);
            db.GroupRequests.Remove(ToDelete);
            db.SaveChanges();
            if (SenderId == _userManager.GetUserId(User))
                return RedirectToAction("Show", "Groups", new { id = GroupId });
            return RedirectToAction("Status", "Groups", new { id = GroupId });
        }
        [HttpPost]
        public IActionResult Accept(string SenderId, int GroupId)
        {
            GroupRequest ToDelete = db.GroupRequests.Find(SenderId, GroupId);
            db.GroupRequests.Remove(ToDelete);
            db.SaveChanges();

            GroupMember ToAdd = new GroupMember();
            ToAdd.GroupId = GroupId;
            ToAdd.UserId = SenderId;
            db.GroupMembers.Add(ToAdd);
            db.SaveChanges();

            return RedirectToAction("Status", "Groups", new { id = GroupId });
        }
    }
}