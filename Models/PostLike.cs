using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models
{
    public class PostLike
    {
        [Key]
        [Column(Order = 0)]
        public int PostId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string? UserId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
