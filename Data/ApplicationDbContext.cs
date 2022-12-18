using ASPace.Areas.Identity.Data;
using ASPace.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPace.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Request>().HasKey(table => new {
            table.SenderId, table.ReceiverId
        });

        builder.Entity<Friendship>().HasKey(table => new {
            table.FirstId, table.SecondId
        });

        builder.Entity<GroupMember>().HasKey(table => new {
            table.UserId, table.GroupId
        });
        builder.Entity<GroupRequest>().HasKey(table => new {
            table.SenderId, table.ReceiverId
        });

        builder.Entity<Request>()
                    .HasOne(m => m.Sender)
                    .WithMany(t => t.Senders)
                    .HasForeignKey(m => m.SenderId);

        builder.Entity<Request>()
                    .HasOne(m => m.Receiver)
                    .WithMany(t => t.Receivers)
                    .HasForeignKey(m => m.ReceiverId);

        builder.Entity<Friendship>()
                    .HasOne(m => m.First)
                    .WithMany(t => t.FirstFriends)
                    .HasForeignKey(m => m.FirstId);

        builder.Entity<Friendship>()
                    .HasOne(m => m.Second)
                    .WithMany(t => t.SecondFriends)
                    .HasForeignKey(m => m.SecondId);

        builder.Entity<Chat>()
                    .HasOne(m => m.Sender)
                    .WithMany(t => t.ChatSenders)
                    .HasForeignKey(m => m.SenderId);

        builder.Entity<Chat>()
                    .HasOne(m => m.Receiver)
                    .WithMany(t => t.ChatReceivers)
                    .HasForeignKey(m => m.ReceiverId);

        builder.Entity<CommentLike>()
                    .HasKey(pt => new { pt.CommentId, pt.UserId });
        builder.Entity<PostLike>()
                    .HasKey(pt => new { pt.PostId, pt.UserId });
        builder.Entity<Chat>()
                    .HasKey(table => new { table.SenderId, table.ReceiverId, table.Time });

        base.OnModelCreating(builder);
     }

    public DbSet<Group> Groups { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<Comment>? Comments { get; set; }
    public DbSet<PostLike>? PostLikes { get; set; }
    public DbSet<CommentLike>? CommentLikes { get; set; }
    public DbSet<GroupRequest>? GroupRequests { get; set; }
    public DbSet<Chat> Chats { get; set; }
}
