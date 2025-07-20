using System.Net.Http.Json;
using System.Text.Json;
using BackendTracker;
using BackendTracker.Auth;
using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Ticket;
using BackendTracker.Ticket.FileUpload;
using BackendTrackerTest.IntegrationTests.IntegrationTestSetup;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Environment = BackendTracker.Ticket.Environment;

namespace BackendTrackerTest.IntegrationTests.Ticket;

public class TicketIntegrationTests(BackendTrackerFactory<Program> factory, ITestOutputHelper testOutputHelper)
    : IClassFixture<BackendTrackerFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private ApplicationUser _user = null!;
    private string _token = null!;

    public async Task InitializeAsync()
    {
        var loginRequest = new LoginRequest
        {
            UserName = "testuser",
            Password = "123abc"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        loginResponse.EnsureSuccessStatusCode();


        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        _token = authResult?.Token ?? throw new Exception("Missing token");
        _user = authResult.User;

        using var scope = factory.Services.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationContext>>();
        await using var context = await dbContextFactory.CreateDbContextAsync();

        await context.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTickets_ShouldReturnTicketsForSubmitter()
    {
                _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var response = await _client.GetAsync($"/api/tickets?submitterId={_user.Id}");

        var body = response.Content.ReadAsStringAsync().Result;
        testOutputHelper.WriteLine(body);

        response.EnsureSuccessStatusCode();

        var tickets = await response.Content.ReadFromJsonAsync<List<BackendTracker.Ticket.Ticket>>();

        Assert.NotEmpty(tickets);
    }

    [Fact]
    public async Task CreateTicket_ShouldCreateAndReturnATicketResponse()
    {
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var response = await _client.PostAsJsonAsync("/api/tickets", new TicketRequestBody 
        {
            Environment = Environment.Device,
            Title = "Test Ticket",
            Description = "This is a test ticket.",
            StepsToReproduce = "1. Do this\n2. Do that",
            ExpectedResult = "Expected outcome",
            SubmitterId = _user.Id,
            Files = new List<TicketFile>(),
            IsResolved = false
        } );

        var body = response.Content.ReadAsStringAsync().Result;
        testOutputHelper.WriteLine(body);

        var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
        
        Assert.NotNull(ticket);
        Assert.Equal(ticket.Title, "Test Ticket");
        Assert.Equal(ticket.SubmitterId, _user.Id);
    }
}