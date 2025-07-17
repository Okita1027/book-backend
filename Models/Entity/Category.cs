using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.Entity;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; } // 分类名称，如 "小说", "计算机科学"

    // --- 导航属性 ---
    // 一个 Category 包含多个 Book (多对多)
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    // 通用属性：创建时间、更新时间
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}