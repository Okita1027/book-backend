using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity;

public class Category : IAuditableEntity // 实现通用属性接口
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; } // 分类名称，如 "小说", "计算机科学"

    // --- 导航属性 ---
    // 一个 Category 包含多个 Book (多对多)
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    // 通用属性：创建时间、更新时间
    [Column(TypeName = "datetime(0)")]
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    //public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}