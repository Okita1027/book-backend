using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;

namespace book_frontend.Services.Interfaces;

public interface IAuthorService
{
    /// <summary>
    /// 获取所有作者
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<List<AuthorVO>>> GetAllAuthorsAsync();

    /// <summary>
    /// 根据ID获取作者
    /// </summary>
    /// <param name="id">作者ID</param>
    /// <returns></returns>
    Task<ApiResponse<AuthorVO>> GetAuthorByIdAsync(int id);

    /// <summary>
    /// 获取作者的图书列表
    /// </summary>
    /// <param name="id">作者ID</param>
    /// <returns></returns>
    Task<ApiResponse<List<BookVO>>> GetAuthorBooksAsync(int id);

    /// <summary>
    /// 添加作者
    /// </summary>
    /// <param name="author">作者信息</param>
    /// <returns></returns>
    Task<ApiResponse<AuthorVO>> AddAuthorAsync(EditAuthorDTO author);

    /// <summary>
    /// 更新作者
    /// </summary>
    /// <param name="id">作者ID</param>
    /// <param name="author">作者信息</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> UpdateAuthorAsync(int id, EditAuthorDTO author);

    /// <summary>
    /// 删除作者
    /// </summary>
    /// <param name="id">作者ID</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeleteAuthorAsync(int id);

    /// <summary>
    /// 批量删除作者
    /// </summary>
    /// <param name="ids">作者ID列表</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeleteAuthorsAsync(List<int> ids);
}