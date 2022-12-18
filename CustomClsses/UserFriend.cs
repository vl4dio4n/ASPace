using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASPace.CustomClasses{
    public class UserFriend{
        public UserFriend(string userName, string userId, DateTime acceptDate){
            UserName = userName;
            UserId = userId;
            AcceptDate = acceptDate; 
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime AcceptDate { get; set; }
    }

}