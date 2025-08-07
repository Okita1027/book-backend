using book_backend.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity
{
    public class Fine : AuditableEntity
    {
        public int Id { get; init; }

        [Column(TypeName = "decimal(18,2)")] // 指定数据库中的数据类型
        public decimal Amount { get; set; } // 罚款金额

        public string Reason { get; set; } = "Overdue book"; // 罚款原因
        
        [Column(TypeName = "datetime(0)")]
        public DateTime? PaidDate { get; set; }

        // --- 导航属性 ---
        public int LoanId { get; set; }
        public Loan Loan { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // 通用属性：创建时间、更新时间
        [Column(TypeName = "datetime(0)")]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
