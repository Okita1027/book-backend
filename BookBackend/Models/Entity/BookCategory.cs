using book_backend.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity
{
    // 复合主键将在 DbContext 中使用 Fluent API 配置
    // [Table("bookcategory")]  // 手动指定表名称
    public class BookCategory : AuditableEntity
    {
        public int BookId { get; init; }
        public Book Book { get; set; }

        public int CategoryId { get; init; }
        public Category Category { get; set; }

        // 通用属性：创建时间、更新时间
        [Column(TypeName = "datetime(0)")]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}