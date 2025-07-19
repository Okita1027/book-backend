using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity
{
    public class Loan : AuditableEntity // 实现通用属性接口
    {
        public int Id { get; set; }

        public DateTime LoanDate { get; set; } // 借出日期
        public DateTime DueDate { get; set; }  // 应还日期
        public DateTime? ReturnDate { get; set; } // 实际归还日期 (nullable, 如果未还则为 null)

        // --- 导航属性 ---
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // 一个借阅记录可能产生一笔罚款
        public Fine? Fine { get; set; }

        // 通用属性：创建时间、更新时间
        [Column(TypeName = "datetime(0)")]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
