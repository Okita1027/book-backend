using book_backend.Models.DTO;
using book_backend.Models.VO;
using book_backend.utils;

namespace book_backend.Services
{
    public interface IBooksService
    {
        Task<IEnumerable<BookVO>> GetAllBooksAsync();
        Task<BookVO?> GetBookByIdAsync(int id);
        Task<IEnumerable<BookVO>> SearchBooksAsync(string title, string isbn, string categoryName, string authorName, string publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd);
        Task<string> LoanBookAsync(int id, string username);
        Task<string> ReturnBookAsync(int id, string username);
        Task<int> AddBookAsync(EditBookDTO bookDTO);
        Task UpdateBookAsync(int id, EditBookDTO bookDTO);
        Task DeleteBookAsync(int id);
        Task DeleteBooksAsync(List<int> ids);
        Task<List<RawBookVO>> GetAllRawBooksAsync();
        Task<Pagination<BookVO>> SearchBooksPaginatedAsync(PaginationRequest paginationRequest, string? title, string? isbn, string? categoryName, string? authorName, string? publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd);
    }
}
