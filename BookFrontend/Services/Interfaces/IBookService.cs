using book_frontend.Models;

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
    /// <param name="searchTerm">搜索关键词</param>
    /// <param name="pageNumber">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页图书列表</returns>
    Task<ApiResponse<PagedResponse<Book>>> SearchBooksAsync(string? searchTerm = null, int pageNumber = 1, int pageSize = 10);
    
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
