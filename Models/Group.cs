using ASPace.Areas.Identity.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASPace.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string? CreatorId { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsPrivate { get; set; }

        public virtual ICollection<Post>? Posts { get; set;}
        public virtual ApplicationUser? Creator { get; set; }
        public virtual ICollection<GroupMember>? GroupMembers { get; set; }
        public virtual ICollection<GroupRequest>? GroupRequests { get; set; }
    }
}
