using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASPace.CustomClasses{
    public class UserRequest{
        public UserRequest(string receiverId){
            ReceiverId = receiverId;
        }
        public string ReceiverId { get; set; }
    }

}