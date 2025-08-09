using System.ComponentModel.DataAnnotations;

namespace book_frontend.Models;

public class Fine
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int LoanId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "罚款金额必须大于0")]
    public decimal Amount { get; set; }

    public string? Reason { get; set; }
    public FineStatus Status { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    // 导航属性
    public User? User { get; set; }
    public Loan? Loan { get; set; }
}

public enum FineStatus
{
    Unpaid = 0, // 未缴纳
    Paid = 1, // 已缴纳
}
