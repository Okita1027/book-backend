using book_backend.Models.DTO;

namespace book_backend.Services;

public interface ILoansService
{
    Task<string> LoanBookAsync(EditLoanDTO editLoanDto);
    Task<string> ReturnBookAsync(int bookId, int userId);
}