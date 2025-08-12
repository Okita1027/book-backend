namespace book_frontend.Models;

public class LoginResponse
{
    public bool IsSuccess { get; init; }
    public string? Token { get; init; }
    public User? User { get; init; }
    public string? ErrorMessage { get; init; }
}
