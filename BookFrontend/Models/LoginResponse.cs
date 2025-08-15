namespace book_frontend.Models;

public class LoginResponse
{
    public bool IsSuccess { get; init; }
    public string? Token { get; init; }
    public User? User { get; init; }
    public string? ErrorMessage { get; init; }
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
