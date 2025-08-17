using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;

namespace book_frontend.Services.Interfaces;

public interface IPublisherService
{
    /// <summary>
    /// 获取所有出版社
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<List<PublisherVO>>> GetAllPublishersAsync();

    /// <summary>
    /// 根据ID获取出版社
    /// </summary>
    /// <param name="id">出版社ID</param>
    /// <returns></returns>
    Task<ApiResponse<PublisherVO>> GetPublisherByIdAsync(int id);

    /// <summary>
    /// 添加出版社
    /// </summary>
    /// <param name="publisher">出版社信息</param>
    /// <returns></returns>
    Task<ApiResponse<PublisherVO>> AddPublisherAsync(EditPublisherDTO publisher);

    /// <summary>
    /// 更新出版社
    /// </summary>
    /// <param name="id">出版社ID</param>
    /// <param name="publisher">出版社信息</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> UpdatePublisherAsync(int id, EditPublisherDTO publisher);

    /// <summary>
    /// 删除出版社
    /// </summary>
    /// <param name="id">出版社ID</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeletePublisherAsync(int id);

    /// <summary>
    /// 批量删除出版社
    /// </summary>
    /// <param name="ids">出版社ID列表</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeletePublishersAsync(List<int> ids);
}