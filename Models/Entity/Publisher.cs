using book_backend.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Models.Entity;

public class Publisher : AuditableEntity
{
    public int Id { get; init; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } // 出版社名称

    // --- 导航属性 ---
    // 一个出版社可以出版多本书
    public ICollection<Book> Books { get; set; } = new List<Book>();

    // 通用属性：创建时间、更新时间
    [Column(TypeName = "datetime(0)")]
    public DateTime CreatedTime { get; init; } = DateTime.Now;
    //public DateTime UpdatedTime { get; set; } = DateTime.Now;
}