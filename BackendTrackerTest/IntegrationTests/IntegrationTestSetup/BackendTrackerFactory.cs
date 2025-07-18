using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTrackerTest.IntegrationTests.IntegrationTestSetup;

public class BackendTrackerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            d.ServiceType.FullName.Contains("ApplicationContext"))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddPooledDbContextFactory<ApplicationContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForTesting"));


            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationContext>>();
            using var db = factory.CreateDbContext();
            db.Database.EnsureCreated();
            SeedData(db);
        });
    }

    private void SeedData(ApplicationContext context)
    {
        if (context.ApplicationUsers.Any()) return;

        var hasher = new PasswordHasher<ApplicationUser>();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "testEmail@test.com",
            Role = "User",
        };

        user.Password = hasher.HashPassword(user, "123abc");

        context.ApplicationUsers.Add(user);
        context.SaveChanges();
    }
}