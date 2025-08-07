using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO
{
    public class EditUserDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // 用户名

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // 邮箱，用于登录

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } // 存储哈希后的密码
    }
}
