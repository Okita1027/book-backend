using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models.Entities;

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int BookId { get; set; }

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public LoanStatus Status { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    // 导航属性
    public User? User { get; set; }
    public Book? Book { get; set; }

    // 计算属性
    public bool IsOverdue => ReturnDate == null && DateTime.Now > DueDate;
    public int OverdueDays => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
}

public enum LoanStatus
{
    Active = 0, // 借阅中
    Returned = 1, // 已归还
    Overdue = 2, // 逾期
}
