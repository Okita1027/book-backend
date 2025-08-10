namespace book_frontend.Models;

public class LoginResponse
{
    public bool IsSuccess { get; init; }
    public string? Token { get; set; }
    public User? User { get; set; }
    public string? ErrorMessage { get; set; }
}
