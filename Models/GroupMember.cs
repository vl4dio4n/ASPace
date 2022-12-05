using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models {
    public class GroupMember {
        public string? UserId { get; set; }
        public int GroupId { get; set; }
        public string? Function { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Group? Group { get; set; }
    }
}