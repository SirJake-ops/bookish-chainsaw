using System.Text;
using BackendTracker.Context;
using BackendTracker.Entities.Message;
using BackendTracker.Graphql;
using BackendTracker.GraphQueries;
using BackendTracker.Ticket;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Environment = System.Environment;

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

        builder.Services.AddScoped<TicketService>();
        builder.Services.AddScoped<Mutation>();
        builder.Services.AddScoped<Query>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

        builder.Services.AddControllers();
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddAuthorization();
        

        var app = builder.Build();

        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGraphQL();

        app.Run();
    }
}