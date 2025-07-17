using DEMO_CRUD.Data;

namespace DEMO_CRUD.Models.Entity
{
    // 复合主键将在 DbContext 中使用 Fluent API 配置
    public class BookCategory : IAuditableEntity // 实现通用属性接口
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // 通用属性：创建时间、更新时间
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}