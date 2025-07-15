using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;

namespace BackendTracker.GraphQueries;

public class Query()
{
    public string Hello() => "Hello From Graph";

    public IEnumerable<Conversation>? GetConversations([Service] ApplicationContext context, Guid conversationId) =>
        context.Conversations.ToList();

    public ApplicationUser CreateUser([Service] ApplicationContext context)
    {
        ApplicationUser applicationUser = new ApplicationUser();
        context.ApplicationUsers.Add(applicationUser);
        return applicationUser;
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