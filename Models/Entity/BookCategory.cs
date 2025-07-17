namespace DEMO_CRUD.Models.Entity
{
    // 复合主键将在 DbContext 中使用 Fluent API 配置
    public class BookCategory
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}