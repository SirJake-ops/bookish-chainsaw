using BackendTracker.Entities.ApplicationUser;

namespace BackendTracker.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}