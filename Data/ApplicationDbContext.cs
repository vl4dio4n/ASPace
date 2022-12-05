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

        base.OnModelCreating(builder);
     }

    public DbSet<Group> Groups { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
}
