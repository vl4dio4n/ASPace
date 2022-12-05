using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models {
    public class Request {
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime RequestDate { get; set; }

        // [ForeignKey("SenderId")]
        public virtual ApplicationUser? Sender { get; set; }
        // [ForeignKey("ReceiverId")]
        public virtual ApplicationUser? Receiver { get; set; }
    }
}