using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ASPace.Models;

namespace ASPace.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public DateTime RegistrationDate { get; set; }
    public DateTime LastActivity { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public bool IsPublic { get; set; }

    public virtual ICollection<Request>? Senders { get; set; }
    public virtual ICollection<Request>? Receivers { get; set; }

    public virtual ICollection<Chat>? ChatSenders { get; set; }
    public virtual ICollection<Chat>? ChatReceivers { get; set; }

    public virtual ICollection<Friendship>? FirstFriends { get; set; }
    public virtual ICollection<Friendship>? SecondFriends { get; set; }

    public virtual ICollection<Group>? Groups { get; set; }
    public virtual ICollection<GroupMember>? GroupMembers { get; set; }

    public virtual ICollection<Post>? Posts { get; set; }
    public virtual ICollection<PostLike>? PostLikes { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<CommentLike>? CommentLikes { get; set; }
    public virtual ICollection<GroupRequest>? GroupRequests { get; set; }

}

