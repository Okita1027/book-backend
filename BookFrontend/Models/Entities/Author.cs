using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "作者姓名不能为空")]
    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    // 导航属性
    public List<Book> Books { get; set; } = [];
}
