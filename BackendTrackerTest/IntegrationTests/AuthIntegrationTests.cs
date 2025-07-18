using System.Net.Http.Json;
using System.Text.Json;
using BackendTracker;
using BackendTracker.Auth;
using BackendTrackerTest.IntegrationTests.IntegrationTestSetup;

namespace BackendTrackerTest.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<BackendTrackerFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(BackendTrackerFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnTokenWhenUserIsValid()
    {
        var loginRequest = new LoginRequest
        {
            UserName = "testuser",
            Password = "123abc"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.EnsureSuccessStatusCode();
        
        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(responseData.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }

}