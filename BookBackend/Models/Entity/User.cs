using book_backend.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity
{
    /**
     *  同时代表 管理员和普通用户
     */
    public class User : AuditableEntity
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // 用户名

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // 邮箱，用于登录

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } // 存储哈希后的密码

        public UserRole Role { get; set; } = UserRole.Member; // 角色 (管理员/普通用户)

        [Column(TypeName = "datetime(0)")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // --- 导航属性 ---
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();


        // 通用属性：创建时间、更新时间
        [Column(TypeName = "datetime(0)")]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }

    public enum UserRole
    {
        Member = 0, // 普通用户
        Admin = 1   // 管理员
    }

}