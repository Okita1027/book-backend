using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity
{
    public class Fine
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")] // 指定数据库中的数据类型
        public decimal Amount { get; set; } // 罚款金额

        public string Reason { get; set; } = "Overdue book"; // 罚款原因
        public DateTime? PaidDate { get; set; } // 缴纳日期 (nullable, 未缴则为 null)

        // --- 导航属性 ---
        public int LoanId { get; set; }
        public Loan Loan { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
