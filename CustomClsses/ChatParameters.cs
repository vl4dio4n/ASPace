using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASPace.CustomClasses{
    public class ChatParameters{
        [BindRequired]
        public string FirstUser { get; set; }
        [BindRequired]
        public string SecondUser { get; set; }
    }

}