namespace book_frontend.Models.VOs;

public class BookVO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public DateTime PublishedDate { get; set; }
    public int Stock { get; set; }
    public int Available { get; set; }

    // 作者的名字
    public string AuthorName { get; set; }

    // 出版社的名字
    public string PublisherName { get; set; }
        
    // 书籍的类别
    public List<string> CategoryNames { get; set; } = [];
}