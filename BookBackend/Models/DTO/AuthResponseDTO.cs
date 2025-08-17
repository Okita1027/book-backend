using book_backend.Models.Entity;

namespace book_backend.Models.DTO;

public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty; // AccessToken
    public string RefreshToken { get; set; } = string.Empty; // RefreshToken

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = nameof(UserRole.Member);

    public DateTime ExpiresAt { get; set; } // AccessToken过期时间
    public DateTime RefreshExpiresAt { get; set; } // RefreshToken过期时间
}