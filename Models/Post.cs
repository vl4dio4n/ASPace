using System;
using System.ComponentModel.DataAnnotations;
using ASPace.Areas.Identity.Data;

namespace ASPace.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Content of the post is required")]
        [MaxLength(5000, ErrorMessage = "{0} can have a max of {1} characters")]
        public string? Content { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int LikeCount { get; set; }
        public int? GroupId { get; set; }

        public virtual Group? Group { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
