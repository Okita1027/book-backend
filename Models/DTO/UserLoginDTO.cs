using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO;

public class UserLoginDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } // 邮箱，用于登录

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } // 存储哈希后的密码
}