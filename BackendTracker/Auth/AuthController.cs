using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BackendTracker.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationContext _applicationContext;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationContext applicationContext, IConfiguration configuration)
    {
        _applicationContext = applicationContext;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = await _applicationContext.ApplicationUsers.FirstOrDefaultAsync(user =>
            user.UserName == loginRequest.UserName)!;
        if (user == null) return Unauthorized("Invalid username.");

        var hasher = new PasswordHasher<ApplicationUser>();
        var result = hasher.VerifyHashedPassword(user, user.Password!, loginRequest.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized("Invalid password.");

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }


    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };
        
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}