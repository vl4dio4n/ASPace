using System;
using System.ComponentModel.DataAnnotations;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public int PostId { get; set; }
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Content of the comment is required")]
        [MaxLength(3000)]
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public int LikeCount { get; set; }

        public virtual Post? Post { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<CommentLike>? CommentLikes { get; set; }
        

    }
}
