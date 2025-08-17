using book_backend.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity
{
    /// <summary>
    /// RefreshToken实体，用于JWT令牌刷新机制
    /// </summary>
    public class RefreshToken : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty; // RefreshToken值

        [Required]
        public int UserId { get; set; } // 关联的用户ID

        [Column(TypeName = "datetime(0)")]
        public DateTime ExpiresAt { get; set; } // 过期时间

        public bool IsRevoked { get; set; } = false; // 是否已撤销

        [MaxLength(50)]
        public string? CreatedByIp { get; set; } // 创建时的IP地址

        [MaxLength(50)]
        public string? RevokedByIp { get; set; } // 撤销时的IP地址

        [Column(TypeName = "datetime(0)")]
        public DateTime? RevokedAt { get; set; } // 撤销时间

        [MaxLength(200)]
        public string? ReasonRevoked { get; set; } // 撤销原因

        // 导航属性
        public User User { get; set; } = null!;

        // 辅助属性
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}