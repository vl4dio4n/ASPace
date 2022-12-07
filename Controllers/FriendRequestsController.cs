using ASPace.Areas.Identity.Data;
using ASPace.Data;
using ASPace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ArticlesApp.Controllers{
    [Authorize]
    public class FriendRequestsController : Controller{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FriendRequestsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        ){
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles="User,Moderator,Admin")]
        public IActionResult Index(string id){
            string userId = _userManager.GetUserId(User);

            if(userId == id){
                if(TempData.ContainsKey("message")){
                    ViewBag.Message = TempData["message"];
                }

                var requests = (from request in db.Requests
                                join user in db.Users on request.SenderId equals user.Id 
                                where request.ReceiverId == userId
                                orderby request.RequestDate descending
                                select new {
                                    UserName = user.UserName,
                                    UserId = user.Id,
                                    RequestDate = request.RequestDate
                                });
                ViewBag.Requests = requests.ToArray();

                var myRequests = (from request in db.Requests
                                join user in db.Users on request.ReceiverId equals user.Id 
                                where request.SenderId == userId
                                orderby request.RequestDate descending
                                select new {
                                    UserName = user.UserName,
                                    UserId = user.Id,
                                    RequestDate = request.RequestDate
                                });
                ViewBag.MyRequests = myRequests.ToArray();
                return View();
            } else {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
        }

        [HttpPost]
        public IActionResult New(string id){
            string userId = _userManager.GetUserId(User);
            Request request = new Request();
            request.SenderId = userId;
            request.ReceiverId = id;
            request.RequestDate = DateTime.Now;
            db.Requests.Add(request);
            db.SaveChanges();
            TempData["message"] = "The friend request has been sent";

            Task<ApplicationUser> t = _userManager.FindByIdAsync(id);
            t.Wait();
            return RedirectToRoute(new {controller = "Users", action =  "Show", id = t.Result.UserName});
        }

        [HttpPost]
        public IActionResult Delete(string id){
            string userId = _userManager.GetUserId(User);
            Request request = db.Requests.Where(r => r.SenderId == id && r.ReceiverId == userId).First();
            db.Requests.Remove(request);
            db.SaveChanges();
            TempData["message"] = "Friend request rejected";
            return RedirectToRoute(new {Controller = "FriendRequests", Action = "Index", id = userId});
        }

        [HttpPost]
        public IActionResult Cancel(string id){
            string userId = _userManager.GetUserId(User);
            Request request = db.Requests.Where(r => r.ReceiverId == id && r.SenderId == userId).First();
            db.Requests.Remove(request);
            db.SaveChanges();
            TempData["message"] = "Friend request canceled";
            return RedirectToRoute(new {Controller = "FriendRequests", Action = "Index", id = userId});
        }
    }
}