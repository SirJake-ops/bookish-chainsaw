using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BackendTracker.Entities.ApplicationUser;

public class ApplicationUser : BaseEntity
{
    [Key] public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public string? Password { get; set; }

    [MaxLength(50)]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public string? Role { get; set; }

    public string? Token { get; set; } 
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateTime LastLoginTime { get; set; }
    public bool IsOnline { get; set; }
}