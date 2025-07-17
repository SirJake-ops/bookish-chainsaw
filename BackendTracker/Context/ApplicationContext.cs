using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
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

    public DbSet<ApplicationUser?> ApplicationUsers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}