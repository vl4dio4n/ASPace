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
    public class FriendshipsController : Controller{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FriendshipsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        ){
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
    
        [HttpPost]
        [Authorize(Roles="User,Moderator,Admin")]
        public IActionResult New(string id){
            string userId = _userManager.GetUserId(User);
            Friendship friendship = new Friendship();
            friendship.FirstId = userId;
            friendship.SecondId = id;
            friendship.AcceptDate = DateTime.Now;
            db.Friendships.Add(friendship);
            db.SaveChanges();
            TempData["message"] = "Friend request accepted";

            Request request = db.Requests.Where(r => r.SenderId == id && r.ReceiverId == userId).First();
            db.Requests.Remove(request);
            db.SaveChanges();

            return RedirectToRoute(new {Controller = "FriendRequests", Action = "Index", id = userId});
        }

        [HttpPost]
        [Authorize(Roles="User,Moderator,Admin")]
        public IActionResult Delete(string id){
            string userId = _userManager.GetUserId(User);
            string userName = _userManager.GetUserName(User);
            Friendship friendship = (from f in db.Friendships
                                    where (f.FirstId == userId && f.SecondId == id) || (f.FirstId == id && f.SecondId == userId)
                                    select f).First();
            db.Friendships.Remove(friendship);
            db.SaveChanges();
            TempData["message"] = "Friend removed successfully";
            return RedirectToRoute(new {Controller = "Users", Action = "Show", id = userName});
        }
    }
}