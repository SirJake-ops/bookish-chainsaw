using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using Microsoft.EntityFrameworkCore;

namespace BackendTrackerTest;

public class EntitiesTests
{
    [Fact]
    public void Test1()
    {
        Assert.True(true);
    }

    [Fact]
    public void CanAddAndRetrieveApplicationUser()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using (var context = new ApplicationContext(options))
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "test user",
                Email = "something@Yahoo.com",
                Password = "password"
            };

            context.ApplicationUsers.Add(user);
            
            context.SaveChanges();
            
            var fetchedUser = context.ApplicationUsers.Find(user.Id);

            Assert.NotNull(fetchedUser);
            Assert.Equal(user, fetchedUser);
            Assert.Equal(user.UserName, fetchedUser.UserName);
            
        }
        
    }
    
    
}