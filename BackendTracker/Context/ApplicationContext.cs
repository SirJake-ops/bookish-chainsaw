using System.Diagnostics.CodeAnalysis;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        DotNetEnv.Env.Load();
    }
#pragma warning restore IL3050

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            optionsBuilder.UseNpgsql($"Host=localhost;Database=BackendTracker;Username={username};Password={password}");
        }

        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}