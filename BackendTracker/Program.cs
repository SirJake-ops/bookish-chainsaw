using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Entities.Message;
using BackendTracker.GraphQueries;

namespace BackendTracker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);


        builder.Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddType<ApplicationUser>()
            .AddType<Message>()
            .AddType<Conversation>();

        var app = builder.Build();

        app.MapGraphQL();

        app.Run();
    }
}