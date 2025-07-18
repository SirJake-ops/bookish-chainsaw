using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
using BackendTracker.Graphql.GraphqlTypes;
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
            .Where(m => m.Id == userId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<ApplicationUser?> GetUser(UserSearchInput searchInput) =>
        await context.ApplicationUsers
            .Where(m => m.UserName == searchInput.UserName
                        && m.Email == searchInput.UserEmail)
            .FirstOrDefaultAsync();

    [Authorize]
    public async Task<IEnumerable<Message>> GetMessagesUser(Guid userId) =>
        await context.Messages
            .Where(m => m.Id == userId)
            .AsNoTracking()
            .ToListAsync();
}