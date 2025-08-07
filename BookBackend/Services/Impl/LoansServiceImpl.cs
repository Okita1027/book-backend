using book_backend.Data;
using book_backend.Exceptions;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using Microsoft.EntityFrameworkCore;
using static book_backend.Constants.IServiceConstants;

namespace book_backend.Services.Impl;

public class LoansServiceImpl(ApplicationDbContext context) : ILoansService
{
    public async Task<string> LoanBookAsync(EditLoanDTO editLoanDto)
    {
        var book = await context.Books.FindAsync(editLoanDto.BookId);
        if (book == null)
        {
            throw new ArgumentException(BOOK_NOT_FOUND);
        }

        if (book.Available <= 0)
        {
            throw new BusinessException(BOOK_NOT_FOUND);
        }

        var user = await context.Users.FindAsync(editLoanDto.UserId);
        if (user == null)
        {
            throw new ArgumentException(USER_NOT_FOUND);
        }

        var loan = new Loan
        {
            BookId = editLoanDto.BookId,
            UserId = editLoanDto.UserId,
            LoanDate = DateTime.Now,
            DueDate = editLoanDto.DueDate > DateTime.Now ? editLoanDto.DueDate : DateTime.Now.AddMonths(1),
            // 初始未归还
            ReturnDate = null
        };
        // 减少该书籍的可用数量
        book.Available -= 1;
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return $"书籍《{book.Title}》已借出给 {user.Name}。";
    }

    public async Task<string> ReturnBookAsync(int bookId, int userId)
    {
        var loan = await context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.User.Id == userId && l.ReturnDate == null);
        if (loan == null)
        {
            throw new ArgumentException(RECORD_NOT_FOUND);
        }

        loan.ReturnDate = DateTime.Now;
        // 超期还书处理
        if (loan.DueDate < loan.ReturnDate)
        {
            // span.Days VS span.TotalDays : 前者是整数，后者带小数
            TimeSpan span = loan.ReturnDate.Value - loan.DueDate;
            int spanDays = span.Days;
            var fine = new Fine
            {
                Amount = 0.5m * spanDays,
                Reason = "超期还书",
                LoanId = loan.Id,
                UserId = userId
            };
            context.Fines.Add(fine);
        }

        // 更新书籍的可用数量
        loan.Book.Available += 1;
        await context.SaveChangesAsync();
        return OPERATION_SUCCESS;
    }
}