using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.Entity;

public class Publisher : IAuditableEntity // 实现通用属性接口
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } // 出版社名称

    // --- 导航属性 ---
    // 一个出版社可以出版多本书
    public ICollection<Book> Books { get; set; } = new List<Book>();

    // 通用属性：创建时间、更新时间
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    //public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}