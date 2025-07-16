using BackendTracker.Context;
using BackendTracker.Entities.Message;
using BackendTracker.GraphQueries;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddPooledDbContextFactory<ApplicationContext>(options =>
            options.UseNpgsql(
                $"Host=localhost;Port=5432;Database={Environment.GetEnvironmentVariable("POSTGRES_DB_NAME_DEV")};Username={Environment.GetEnvironmentVariable("POSTGRES_USERNAME_DEV")};Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD_DEV")}"));


        builder.Services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddType<Message>()
            .AddType<Conversation>();

        builder.Services.AddScoped<Mutation>();
        builder.Services.AddScoped<Query>();

        var app = builder.Build();

        app.MapGraphQL();

        app.Run();
    }
}