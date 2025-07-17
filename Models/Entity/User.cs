using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.Entity
{
    /**
     *  同时代表 管理员和普通用户
     */

    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // 用户名

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // 邮箱，用于登录

        [Required]
        public string PasswordHash { get; set; } // 存储哈希后的密码，绝不存明文

        public UserRole Role { get; set; } = UserRole.Member; // 角色 (管理员/普通用户)

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // --- 导航属性 ---
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }

    public enum UserRole
    {
        Member, // 普通用户
        Admin   // 管理员
    }

}