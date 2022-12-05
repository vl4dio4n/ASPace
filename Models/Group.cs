using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models
{
    public class Group{
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CreatorId { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual ApplicationUser? Creator { get; set; }
        public virtual ICollection<GroupMember>? GroupMembers { get; set; }
    }    
}