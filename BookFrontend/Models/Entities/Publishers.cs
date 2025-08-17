using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models.Entities;

public class Publisher
{
    public int Id { get; set; }

    [Required(ErrorMessage = "出版社名称不能为空")]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    // 导航属性
    public List<Book> Books { get; set; } = [];
}
