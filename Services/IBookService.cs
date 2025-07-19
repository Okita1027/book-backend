using DEMO_CRUD.Models.DTO;

namespace DEMO_CRUD.Services;

public interface IBooksService
{
    Task<IEnumerable<BookDTO>> GetBooksAsync();
    Task<BookDTO?> GetBookByIdAsync(int id);

    Task<IEnumerable<BookDTO>> SearchBooksAsync(
        string? title, string? isbn, string? authorName,
        string? publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd);

    Task<(bool Success, string Message)> LoanBookAsync(int id, string username);
    Task<(bool Success, string Message)> ReturnBookAsync(int id, string username);
    Task<(bool Success, string Message, int? BookId)> AddBookAsync(EditBookDTO bookDTO);
    Task<(bool Success, string Message)> UpdateBookAsync(int id, EditBookDTO bookDTO);
    Task<bool> DeleteBookAsync(int id);
}