using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models {
    public class Chat {
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime Time { get; set; }
        public string? Message {get; set; }
        public bool Seen { get; set; }

        public virtual ApplicationUser? Sender { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }
    }
}