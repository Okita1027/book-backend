using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity
{
    /**
     *  同时代表 管理员和普通用户
     */
    public class User : IAuditableEntity // 实现通用属性接口
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
        [MaxLength(255)]
        public string PasswordHash { get; set; } // 存储哈希后的密码

        public UserRole Role { get; set; } = UserRole.Member; // 角色 (管理员/普通用户)

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // --- 导航属性 ---
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();


        // 通用属性：创建时间、更新时间
        [Column(TypeName = "datetime(0)")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }

    public enum UserRole
    {
        Member, // 普通用户
        Admin   // 管理员
    }

}