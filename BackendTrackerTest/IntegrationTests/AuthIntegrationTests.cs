using BackendTracker;
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
    public void ShouldBeTrue()
    {
        var number = 42;
        Assert.Equal(42, number);
    }

}