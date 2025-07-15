using System.Diagnostics.CodeAnalysis;
using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;

namespace BackendTracker.GraphQueries;

public class Query()
{
    public string Hello() => "Hello From Graph";

    public IEnumerable<Conversation>? GetConversations([Service] ApplicationContext context) =>
        context.Conversations.ToList();

    public ApplicationUser? CreateUser([Service] ApplicationContext context,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        string userName,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        string email,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        string password,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        string role)
    {
        ApplicationUser applicationUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            Password = password,
            Role = role,
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.Now.AddHours(24),
            LastLoginTime = DateTime.Now,
            IsOnline = false
        };
        try
        {
            context.ApplicationUsers.Add(applicationUser);
            return applicationUser;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public IEnumerable<ApplicationUser> GetUsers([Service] ApplicationContext context, Guid userId) =>
        context.ApplicationUsers.ToList();

    public IEnumerable<Message> GetMessages([Service] ApplicationContext context, Guid messageId) =>
        context.Messages.ToList();

    public Conversation? CreateConversation([Service] ApplicationContext context)
    {
        try
        {
            var conversation = new Conversation();
            context.Conversations.Add(conversation);
            return conversation;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
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