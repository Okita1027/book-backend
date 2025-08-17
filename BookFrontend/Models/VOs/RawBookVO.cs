namespace book_frontend.Models.VOs;

public class RawBookVO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public DateTime PublishedDate { get; set; }
    public int Stock { get; set; }
    public int Available { get; set; }

    public int AuthorId { get; set; }
    public string AuthorName { get; set; }

    public int PublisherId { get; set; }
    public string PublisherName { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
        
    // 书籍的类别ID与名称
    public Dictionary<int, string> CategoryDictionary { get; set; } = [];
}