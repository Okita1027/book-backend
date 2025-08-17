using book_frontend.Models.DTOs;
using book_frontend.Models.Entities;

namespace book_frontend.Services.Interfaces;

public interface IBookService
{
    /// <summary>
    /// 获取所有图书
    /// </summary>
    /// <returns>图书列表</returns>
    Task<ApiResponse<List<Book>>> GetAllBooksAsync();
    
    /// <summary>
    /// 根据ID获取图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <returns>图书信息</returns>
    Task<ApiResponse<Book>> GetBookByIdAsync(int id);
    

    /// <summary>
    /// 分页搜索图书
    /// </summary>
    /// <param name="title"></param>
    /// <param name="author"></param>
    /// <param name="category"></param>
    /// <param name="publisher"></param>
    /// <param name="isbn"></param>
    /// <param name="publishDateStart"></param>
    /// <param name="publishDateEnd"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns>分页图书列表</returns>
    Task<ApiResponse<PagedResponse<Book>>> SearchBooksAsync(
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
    /// <returns>添加结果</returns>
    Task<ApiResponse<Book>> AddBookAsync(Book book);
    
    /// <summary>
    /// 更新图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <param name="book">图书信息</param>
    /// <returns>更新结果</returns>
    Task<ApiResponse<Book>> UpdateBookAsync(int id, Book book);
    
    /// <summary>
    /// 删除图书
    /// </summary>
    /// <param name="id">图书ID</param>
    /// <returns>删除结果</returns>
    Task<ApiResponse<bool>> DeleteBookAsync(int id);
}
