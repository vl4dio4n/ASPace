using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPace.Data;
using ASPace.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ASPace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASPace.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupsController(
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
            var groups = db.Groups.Include("Creator").OrderByDescending(a => a.Date);

            // Search 
            string search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null){
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim().ToLower();
            }

            List<int> groupIds = db.Groups.Where(
                group => group.Title.ToLower().Contains(search)
                || group.Description.ToLower().Contains(search)
                ).Select(g => g.Id).ToList();

            groups = groups.Where(group => groupIds.Contains(group.Id)).OrderByDescending(g => g.Date);

            ViewBag.SearchString = search;
            ViewBag.Groups = groups;
            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult New()
        {
            Group group = new Group();
            group.CreatorId = _userManager.GetUserId(User);
            group.Title = "";
            group.IsPrivate = false;
            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult New([FromForm] Group group)
        {
            group.CreatorId = _userManager.GetUserId(User);
            group.Date = DateTime.Now;
            try
            {
                db.Groups.Add(group);
                db.SaveChanges();
                GroupMember groupmember = new GroupMember();
                groupmember.GroupId = group.Id;
                groupmember.UserId = group.CreatorId;
                groupmember.Function = "admin";
                db.GroupMembers.Add(groupmember);
                db.SaveChanges();
                TempData["message"] = "The group has been created!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(group);
            }
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Group? group = db.Groups.Where(m => m.Id == id).Include("Posts.User").Include("Creator")
                .Include("GroupMembers").Include("GroupRequests").First();
            if (group == null)
            {
                TempData["message"] = "The group with ID " + id.ToString() + " doesn't exist!";
                return RedirectToAction("Index");
            }
            ViewBag.CurrentUser = new Tuple<string, bool>(_userManager.GetUserId(User), group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"));
            ViewBag.IsMember = db.GroupMembers.Find(_userManager.GetUserId(User), id) != null;
            ViewBag.SentRequest = db.GroupRequests.Find(_userManager.GetUserId(User), id) != null;
            ViewBag.Members = db.GroupMembers.Where(m => m.GroupId == group.Id).Include("User").ToList();
            return View(group);

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult Edit(int id)
        {
            Group? group = db.Groups.Where(m => m.Id == id)
                        .Include("Creator").Include("GroupMembers").Include("Posts")
                        .Include("GroupRequests")
                        .First();
            if (group == null)
            {
                TempData["message"] = "The group with ID " + id.ToString() + " doesn't exist!";
                return RedirectToAction("Index");
            }
            if (group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(group);
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to modify this group!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Group? group = db.Groups.Where(m => m.Id == id)
                        .Include("Creator").Include("GroupMembers").Include("Posts")
                        .Include("GroupRequests")
                        .First();
                    if (group == null)
                    {
                        TempData["message"] = "The group with ID " + id.ToString() + " doesn't exist!";
                        return RedirectToAction("Index");
                    }
                    if (group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                    {
                        group.Title = requestGroup.Title;
                        group.Description = requestGroup.Description;
                        group.Date = requestGroup.Date;
                        db.SaveChanges();
                        TempData["message"] = "The group has been successfully changed!";
                        return RedirectToAction("Show", new { id = group.Id });
                    }
                    else
                    {
                        TempData["message"] = "You don't have enough permissions to modify this Group!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestGroup);
                }
            }
            catch (Exception e)
            {
                return View(requestGroup);
            }
        }

        public IActionResult Status(int id)
        {
            Group group = db.Groups.Where(m => m.Id == id)
                        .Include("Creator").Include("GroupMembers.User").Include("Posts")
                        .Include("GroupRequests.Sender")
                        .First();
            if (_userManager.GetUserId(User) == group.CreatorId)
            {
                ViewBag.CurrentUser = new Tuple<string, bool>(_userManager.GetUserId(User), group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"));
                return View(group);
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to view this page!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult Delete(int id)
        {
            Group? group = db.Groups.Where(m => m.Id == id)
                        .Include("Creator").Include("GroupMembers").Include("Posts")
                        .Include("GroupRequests")
                        .First(); ;
            if (group == null)
            {
                TempData["message"] = "The group with ID " + id.ToString() + " doesn't exist!";
                return RedirectToAction("Index");
            }
            if (group.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {

                if (group.CreatorId != _userManager.GetUserId(User) && (User.IsInRole("Admin")))
                {

                    string subject = "Your group has been deleted.";
                    string body = "Hello, " + group.Creator.UserName + " ! <br /> Unfortunately, your group " + group.Title + " was deleted by our Admin: " + _userManager.GetUserName(User) + "<br /> :(";

                    // de trimis mail aici cu mesajul de mai sus
                }
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "The group was deleted";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to modify this post!";
                return RedirectToAction("Index");
            }
        }
    }
}