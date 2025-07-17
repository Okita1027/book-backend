using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.Entity;

public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } // 作者名

    public string? Biography { get; set; } = string.Empty;  // 作者简介 (可选)

    // --- 导航属性 ---
    // 一个作者可以写多本书
    public ICollection<Book> Books { get; set; } = new List<Book>();
}