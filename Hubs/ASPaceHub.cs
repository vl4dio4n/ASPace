using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using ASPace.Models;
using ASPace.Data;


namespace ASPace.Hubs{
    [Authorize]
    public class ASPaceHub : Hub {
        private readonly ApplicationDbContext db;
        private static Dictionary<string, List<string>> ConnectedUsers = new Dictionary<string, List<string>>();

        public ASPaceHub(ApplicationDbContext context){
            db = context;
        }

        public override Task OnConnectedAsync(){
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            if(!ConnectedUsers.ContainsKey(Context.User.Identity.Name)){
                ConnectedUsers.Add(Context.User.Identity.Name, new List<string>());
            }
            ConnectedUsers[Context.User.Identity.Name].Add(Context.ConnectionId);
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers[Context.User.Identity.Name].Remove(Context.ConnectionId);
            if(ConnectedUsers[Context.User.Identity.Name].Count() == 0){
                ConnectedUsers.Remove(Context.User.Identity.Name);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public Task SendMessageToGroup(string receiver, string message){
            IList<string> groups = new List<string> ();
            SaveMessage(Context.User.Identity.Name, receiver, message);
            groups.Add(Context.User.Identity.Name);
            groups.Add(receiver);
            return Clients.Groups(groups).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }

        private void SaveMessage(string sender, string receiver, string message){
            Chat chat = new Chat();
            chat.SenderId = (from user in db.Users where user.UserName == sender select user.Id).First();
            chat.ReceiverId = (from user in db.Users where user.UserName == receiver select user.Id).First();
            chat.Time = DateTime.Now;
            chat.Message = message;
            chat.Seen = ConnectedUsers.ContainsKey(receiver);
            db.Chats.Add(chat);
            db.SaveChanges();
        }
    }
}