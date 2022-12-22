using ASPace.Areas.Identity.Data;
using ASPace.Data;
using ASPace.Models;
using ASPace.CustomClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ArticlesApp.Controllers{

    public class UsersController : Controller{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        ){
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles="User,Moderator,Admin")]
        public IActionResult Show(string id){
            Task<ApplicationUser> t = _userManager.GetUserAsync(User);
            t.Wait();
            var user = t.Result;
            ViewBag.CurrentUser = user.UserName;
         
            if(TempData.ContainsKey("message")){
                ViewBag.Message = TempData["message"];
            }

            if(!db.Users.Any(user => user.UserName == id)){
                TempData["message"] = $"User {id} doesn't exist";
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            
            SetUserProfile(id);
            
            bool isFriend = false;
            for(int i = 0; i < ViewBag.Friends.Length; i ++)
                if(ViewBag.Friends[i].UserId == user.Id)
                    isFriend = true; 

            if(user.UserName == id || isFriend || ViewBag.IsPublic){
                return View();
            } else {
                TempData["message"] = "This profile is private.";
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        private void SetUserProfile(string username){
            Task<ApplicationUser> t =  _userManager.FindByNameAsync(username);
            t.Wait();
            var user = t.Result;
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.UserName = user.UserName;
            ViewBag.Description = user.Description;
            ViewBag.UserId = user.Id;
            ViewBag.IsPublic = user.IsPublic;

            ViewBag.Age = (DateTime.Now - user.BirthDate).Days / 365;

            string[] months = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
            string[] suffixes = {"st", "nd", "rd", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th", "th", "st"};
            ViewBag.BirthMonth = months[user.BirthDate.Month - 1];
            ViewBag.BirthDay = user.BirthDate.Day;
            ViewBag.BirthDaySuffix = suffixes[user.BirthDate.Day - 1];
            
            ViewBag.RegistrationMonth = months[user.RegistrationDate.Month - 1];
            ViewBag.RegistrationDay = user.RegistrationDate.Day;
            ViewBag.RegistrationDaySuffix = suffixes[user.RegistrationDate.Day - 1];

            TimeSpan interval = DateTime.Now - user.LastActivity;
            if(interval.Days > 30){
                ViewBag.Interval = interval.Days / 30;
                ViewBag.UM = "month";
            } else if(interval.Days >= 1){
                ViewBag.Interval = interval.Days;
                ViewBag.UM = "days";
            } else if(interval.Hours >= 1){
                ViewBag.Interval = interval.Hours;
                ViewBag.UM = "hours";
            } else if(interval.Minutes >= 1){
                ViewBag.Interval = interval.Minutes;
                ViewBag.UM = "minutes";
            } else {
                ViewBag.Interval = interval.Seconds;
                ViewBag.UM = "seconds";
            }

            ViewBag.Friends = GetUserFriends(user.Id).ToArray();
            ViewBag.MyFriends = GetUserFriends(_userManager.GetUserId(User)).ToArray();
            ViewBag.MyRequests = GetUserRequests(_userManager.GetUserId(User)).ToArray();
            ViewBag.Groups = GetUserGroups(user.Id).ToArray();
        }

        private IEnumerable<Group> GetUserGroups(string id){
            IEnumerable<Group> groups = (from gm in db.GroupMembers
                            join g in db.Groups on gm.GroupId equals g.Id
                            where gm.UserId == id
                            select g).Include("Creator");
    
            return groups;
        }
        
        private IEnumerable<UserFriend> GetUserFriends(string id){
            IEnumerable<UserFriend> friends = (from friend in db.Friendships
                            join user in db.Users on friend.SecondId equals user.Id
                            where friend.FirstId == id
                            orderby friend.AcceptDate descending
                            select new UserFriend(user.UserName, user.Id, friend.AcceptDate)).AsEnumerable<UserFriend>()
                            .Union(from friend in db.Friendships
                                join user in db.Users on friend.FirstId equals user.Id
                                where friend.SecondId == id
                                orderby friend.AcceptDate descending
                                select new UserFriend(user.UserName, user.Id, friend.AcceptDate));
            return friends;
        }

        private IEnumerable<UserRequest> GetUserRequests(string id){
            var requests = (from request in db.Requests
                            where request.SenderId == id
                            select new UserRequest(request.ReceiverId)).AsEnumerable<UserRequest>()
                            .Union(from request in db.Requests
                                where request.ReceiverId == id
                                select new UserRequest(request.SenderId)
                            );
            return requests;
        }

        [HttpPost]
        public JsonResult Search(string str){
            str = str.ToLower();
    
            List<UserInfo> users = (from user in db.Users 
                        where user.UserName.ToLower().Contains(str) 
                            || (user.FirstName + user.LastName).ToLower().Contains(str) 
                            || (user.LastName + user.FirstName).ToLower().Contains(str)
                        select new UserInfo(user.UserName, user.Id, user.FirstName, user.LastName)).ToList<UserInfo>();

            if(_userManager.GetUserId(User) != null){
                List<UserFriend> friends = GetUserFriends(_userManager.GetUserId(User)).ToList<UserFriend>();
                List<UserRequest> requests = GetUserRequests(_userManager.GetUserId(User)).ToList<UserRequest>();
                foreach(UserInfo user in users){
                    user.IsFriend = friends.Any(friend => user.UserId == friend.UserId);
                    user.SentRequest = requests.Any(request => user.UserId == request.ReceiverId);
                    if(user.UserId == _userManager.GetUserId(User)){
                        user.IsFriend = true;
                        user.SentRequest = true;
                    }
                }
            }

            return Json(new {Users = users, IsSignedIn = (_userManager.GetUserId(User) != null)});
        }
    }
}
