using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models {
    public class GroupRequest {
        public string? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public DateTime RequestDate { get; set; }

        public virtual ApplicationUser? Sender { get; set; }
        public virtual Group? Receiver { get; set; }
    }
}