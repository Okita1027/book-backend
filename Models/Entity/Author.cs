using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity;

public class Author : AuditableEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; } // 作者名

    public string? Biography { get; set; } = string.Empty;  // 作者简介 (可选)

    // --- 导航属性 ---
    // 一个作者可以写多本书
    public ICollection<Book> Books { get; set; } = new List<Book>();


    // 通用属性：创建时间、更新时间
    [Column(TypeName = "datetime(0)")]
    public DateTime CreatedTime { get; init; } = DateTime.Now;
    //public DateTime UpdatedTime { get; set; } = DateTime.Now;

    public DateTime UpdatedTime { get; set; }
}