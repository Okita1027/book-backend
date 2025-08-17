using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models.Entities;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "用户名不能为空")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于6")]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}

public enum UserRole
{
    User = 0,
    Admin = 1,
}
