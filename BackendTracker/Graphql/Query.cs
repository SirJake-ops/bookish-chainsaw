using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
using BackendTracker.GraphQueries.GraphqlTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker.Graphql;

/**
 * TODO: Move the methods that are altering data to a new Mutation class Below
 */
public class Query
{
    private readonly ApplicationContext context;

    public Query(IDbContextFactory<ApplicationContext> dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    public string Hello() => "Hello From Graph";

    [Authorize]
    public async Task<IEnumerable<Conversation>> GetConversations(Guid userId) =>
        await context.Conversations
            .Where(m => m.InitialReceiverId == userId || m.InitialSenderId == userId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<ApplicationUser?> GetUser(Guid userId) =>
        await context.ApplicationUsers
            .Where(m => m.Id == userId)
            .FirstOrDefaultAsync();

    [Authorize]
    public async Task<IEnumerable<Message>> GetMessages(Guid userId) =>
        await context.Messages
            .Where(m => m.ReceiverId == userId && m.SenderId == userId)
            .AsNoTracking()
            .ToListAsync();
}

public class Mutation
{
    private readonly IDbContextFactory<ApplicationContext> _contextFactory;

    public Mutation(IDbContextFactory<ApplicationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }


    // public Conversation? CreateConversation()
    // {
    //     try
    //     {
    //         var conversation = new Conversation();
    //         context.Conversations.Add(conversation);
    //         return conversation;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return null;
    //     }
    // }

    public async Task<ApplicationUser> CreateUser(ApplicationUserInput user)
    {
        await using var context = _contextFactory.CreateDbContext();
        try
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role ?? "User",
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.Now.AddHours(24).ToUniversalTime(),
                LastLoginTime = DateTime.Now.ToUniversalTime(),
                IsOnline = false
            };

            applicationUser.Password = hasher.HashPassword(applicationUser,
                user.Password ?? throw new InvalidOperationException("Password cannot be null"));

            context.ApplicationUsers.Add(applicationUser);
            await context.SaveChangesAsync();
            return applicationUser;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public class ApplicationUserType : ObjectType<ApplicationUser>
{
}

public class ConversationType : ObjectType<Conversation>
{
}

public class MessageType : ObjectType<Message>
{
}