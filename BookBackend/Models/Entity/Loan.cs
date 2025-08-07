using book_backend.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity
{
    public class Loan : AuditableEntity
    {
        public int Id { get; init; }

        /// <summary>
        /// 借出日期
        /// </summary>
        [Column(TypeName = "datetime(0)")]
        public DateTime LoanDate { get; set; }
        /// <summary>
        /// 应还日期
        /// </summary>
        [Column(TypeName = "datetime(0)")]
        public DateTime DueDate { get; set; }
        /// <summary>
        /// 实际归还日期
        /// </summary>
        [Column(TypeName = "datetime(0)")]
        public DateTime? ReturnDate { get; set; }

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
