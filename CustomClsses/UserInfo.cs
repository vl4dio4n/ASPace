using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASPace.CustomClasses{
    public class UserInfo{
        public UserInfo(string userName, string userId, string? firstName, string? lastName){
            UserName = userName;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            IsFriend = false;
            SentRequest = false;
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsFriend { get; set; }
        public bool SentRequest { get; set; }
    }

}