namespace book_frontend.Models.VOs;

public class LoanVO
{
    // 书籍名称
    public string Title { get; set; }
    // 用户名称
    public string Username { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReturnDate { get; set; }
}