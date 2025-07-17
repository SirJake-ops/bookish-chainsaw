using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Graphql;
using BackendTracker.GraphQueries.GraphqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTrackerTest.GrahpqlTests;


public class MutationTestFixture
{
   public readonly IDbContextFactory<ApplicationContext> ContextFactory;
   
   public MutationTestFixture()
   {
      var services = new ServiceCollection();
      
      services.AddDbContextFactory<ApplicationContext>(options =>
      {
        options.UseInMemoryDatabase("GraphqlTestDb" + Guid.NewGuid()); 
      });
      
      ContextFactory = services.BuildServiceProvider().GetRequiredService<IDbContextFactory<ApplicationContext>>();
      
      SeedTestData();
   }
   
   private void SeedTestData()
   {
      using var context = ContextFactory.CreateDbContext();

      context.ApplicationUsers.Add(new ApplicationUser
      {
         Id = Guid.NewGuid(),
         UserName = "TEST_USER",
         Email = "testEmail@yahoo.com",
         Password = "testPassword"
      });
      
      context.SaveChanges();
   }
   
}


public class MutationTest : IClassFixture<MutationTestFixture>
{
   private readonly IDbContextFactory<ApplicationContext> _contextFactory;
   
   public MutationTest(MutationTestFixture fixture)
   {
      _contextFactory = fixture.ContextFactory;
   }
   
   [Fact]
   public async void CreateUser_ShouldCreateNewUser()
   {
      var mutation = new Mutation(_contextFactory);

      var newUser = new ApplicationUserInput
      {
         UserName = "NewUser",
         Email = "newUserEmail@test.com",
         Password = "123Password",
         Role = "User"
      };

      var createdUser = await mutation.CreateUser(newUser);
      
      Assert.NotNull(createdUser);
      Assert.Equal(newUser.UserName, createdUser.UserName);


   }
}