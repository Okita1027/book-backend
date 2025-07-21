using DEMO_CRUD.Models.DTO;

namespace DEMO_CRUD.Services;

public interface ILoansService
{
    Task<string> LoanBookAsync(EditLoanDTO editLoanDto);
    Task<string> ReturnBookAsync(int bookId, int userId);
}