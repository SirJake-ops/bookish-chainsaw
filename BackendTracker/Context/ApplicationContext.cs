using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
using BackendTracker.Ticket.FileUpload;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
#pragma warning restore IL3050

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER_DEV");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_DEV");
            optionsBuilder.UseNpgsql(
                $"Host=localhost;Port=5432;Database=tracker;Username={username};Password={password}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Find a workaround this maybe need to adjust naming of props in Ticket and ApplicationUser
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket.Ticket>()
            .HasOne(t => t.Submitter)
            .WithMany(u => u.SubmittedTickets)
            .HasForeignKey(t => t.SubmitterId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Ticket.Ticket>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Ticket.Ticket> Tickets { get; set; }
    public DbSet<TicketFile> TicketFiles { get; set; }
}