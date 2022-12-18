using ASPace.Areas.Identity.Data;
using ASPace.Data;
using ASPace.CustomClasses;
using ASPace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;



namespace ArticlesApp.Controllers{
    [Authorize]
    public class ChatsController : Controller{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ChatsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        ){
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Show([FromQuery] ChatParameters parameters){
            if(CheckChatParameters(parameters)){
                GetMessages(parameters);
                GetFriends(parameters.SecondUser);
                SetMessagesAsSeen(parameters);
                return View();
            } else {
                TempData["message"] = "Mind your bussiness. You don't have access to this chat.";
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        public IActionResult Index(){
            GetFriends(null);
            ViewBag.FirstUser = _userManager.GetUserName(User);
            return View();
        }

        private void GetMessages(ChatParameters parameters){
            string senderId = _userManager.GetUserId(User);
            string receiverId = (from user in db.Users where user.UserName == parameters.SecondUser select user.Id).First();
            var messages = (from chat in db.Chats
                                        where (chat.SenderId == senderId && chat.ReceiverId == receiverId) || 
                                        (chat.SenderId == receiverId && chat.ReceiverId == senderId)
                                        orderby chat.Time 
                                        select new {SenderId = chat.SenderId, ReceiverId = chat.ReceiverId, Seen = chat.Seen, Message = chat.Message, Time = chat.Time}).ToList();
            ViewBag.CurrentUserId = senderId;
            ViewBag.FirstUser = parameters.FirstUser;
            ViewBag.SecondUser = parameters.SecondUser;
            ViewBag.Messages = messages;
        }

        private void GetFriends(string? secondUser){
            string userId = _userManager.GetUserId(User);
            var friends = (from fs in db.Friendships
            join user in db.Users on fs.SecondId equals user.Id
            where fs.FirstId == userId
            select new {
                UserId = fs.SecondId,
                UserName = user.UserName
            }).Union(from fs in db.Friendships
                join user in db.Users on fs.FirstId equals user.Id
                where fs.SecondId == userId
                select new {
                    UserId = user.Id,
                    UserName = user.UserName
                }).ToList();
            
            List<ChatInfo> friendsInfo = new List<ChatInfo>();
            foreach(var friend in friends){
                bool haveTalked = (from chat in db.Chats
                        where (chat.ReceiverId == userId && chat.SenderId == friend.UserId) || (chat.ReceiverId == friend.UserId && chat.SenderId == userId)
                        select '*').Count() > 0;
                if(haveTalked){
                    DateTime time = (from chat in db.Chats
                            where (chat.SenderId == userId && chat.ReceiverId == friend.UserId) || (chat.ReceiverId == userId && chat.SenderId == friend.UserId)
                            select chat.Time).Max();
        
                    int unreadMessages = (from chat in db.Chats
                            where chat.ReceiverId == userId && chat.SenderId == friend.UserId && !chat.Seen
                            select '*').Count();

                    string lastMessage = (from chat in db.Chats
                                where ((chat.ReceiverId == userId && chat.SenderId == friend.UserId) || (chat.SenderId == userId && chat.ReceiverId == friend.UserId)) && chat.Time == time
                                select chat.Message).First();
                    
                    friendsInfo.Add(new ChatInfo{Time = time, LastMessage = lastMessage, UnreadMessages = unreadMessages, UserName = friend.UserName}); 

                    if(friend.UserName == secondUser){
                        ViewBag.unreadMessages = unreadMessages;
                    }
                } else {
                    friendsInfo.Add(new ChatInfo{UserName = friend.UserName});
                }
            }
            friendsInfo.Sort((x, y) => -x.Time.CompareTo(y.Time));
            ViewBag.FriendsInfo = friendsInfo;
        }

        private void SetMessagesAsSeen(ChatParameters parameters){
            string receiverId = _userManager.GetUserId(User);
            string senderId = (from user in db.Users where user.UserName == parameters.SecondUser select user.Id).First();
            List<Chat> messages = (from chat in db.Chats 
                                where chat.ReceiverId == receiverId && chat.SenderId == senderId && !chat.Seen
                                select chat).ToList();
            foreach(Chat chat in messages){
                chat.Seen = true;
            }
            db.SaveChanges();
        }

        private bool CheckChatParameters(ChatParameters parameters){
            string userName = _userManager.GetUserName(User);
            if(parameters.FirstUser != userName)
                return false;
            string firstUserId = _userManager.GetUserId(User);
            IQueryable<string> query = (from user in db.Users where user.UserName == parameters.SecondUser select user.Id);
            if(query.Count() == 0)
                return false;
            string secondUserId = query.First();
            int friendship = (from fs in db.Friendships
                                where (fs.FirstId == firstUserId && fs.SecondId == secondUserId) || (fs.SecondId == firstUserId && fs.FirstId == secondUserId)
                                select fs).Count();
            return friendship > 0;
        }

    }
}