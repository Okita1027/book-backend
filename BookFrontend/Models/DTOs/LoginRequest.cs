using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "邮箱不能为空")]
    public string Email { get; init; } = string.Empty;
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; init; } = string.Empty;
}
