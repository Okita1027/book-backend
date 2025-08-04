using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Models.VO;

namespace DEMO_CRUD.Services
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
    }
}
