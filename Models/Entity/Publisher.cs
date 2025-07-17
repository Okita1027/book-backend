using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.Entity;

public class Publisher
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } // 出版社名称

    // --- 导航属性 ---
    // 一个出版社可以出版多本书
    public ICollection<Book> Books { get; set; } = new List<Book>();
}