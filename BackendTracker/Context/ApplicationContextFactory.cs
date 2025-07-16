using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BackendTracker.Context;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        Env.Load();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

        var username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME_DEV");
        // var username = "postgres"; // Fallback for development
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_DEV");
        // var password = "postgres"; // Fallback for development
        optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;Database=tracker;Username={username};Password={password}");

        return new ApplicationContext(optionsBuilder.Options);
    }
}