using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models.Entities;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "书名不能为空")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN不能为空")]
    public string Isbn { get; set; } = string.Empty;

    public DateTime PublishedDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "库存不能为负数")]
    public int Stock { get; set; }

    public int Available { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string PublisherName { get; set; } = string.Empty;

    public List<string> CategoryNames { get; set; } = [];
}
