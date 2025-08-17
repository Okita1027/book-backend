using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;

namespace book_frontend.Services.Interfaces;

public interface IBookService
{
    /// <summary>
    /// 获取所有图书
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<List<BookVO>>> GetAllBooksAsync();

    /// <summary>
    /// 根据ID获取图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <returns></returns>
    Task<ApiResponse<BookVO>> GetBookByIdAsync(int id);

    /// <summary>
    /// 搜索图书（分页）
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="author">作者</param>
    /// <param name="category">分类</param>
    /// <param name="publisher">出版社</param>
    /// <param name="isbn">ISBN</param>
    /// <param name="publishDateStart">出版日期开始</param>
    /// <param name="publishDateEnd">出版日期结束</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <returns></returns>
    Task<ApiResponse<PagedResponse<BookVO>>> SearchBooksAsync(
        string? title = null,
        string? author = null,
        string? category = null,
        string? publisher = null,
        string? isbn = null,
        DateTime? publishDateStart = null,
        DateTime? publishDateEnd = null,
        int pageIndex = 1,
        int pageSize = 12
    );

    /// <summary>
    /// 添加图书
    /// </summary>
    /// <param name="book">图书信息</param>
    /// <returns></returns>
    Task<ApiResponse<BookVO>> AddBookAsync(EditBookDTO book);

    /// <summary>
    /// 更新图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <param name="book">图书信息</param>
    /// <returns></returns>
    Task<ApiResponse<BookVO>> UpdateBookAsync(int id, EditBookDTO book);

    /// <summary>
    /// 删除图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeleteBookAsync(int id);
}
