using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;

namespace book_frontend.Services.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// 获取所有分类
    /// </summary>
    /// <returns></returns>
    Task<ApiResponse<List<CategoryVO>>> GetAllCategoriesAsync();

    /// <summary>
    /// 根据ID获取分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns></returns>
    Task<ApiResponse<CategoryVO>> GetCategoryByIdAsync(int id);

    /// <summary>
    /// 添加分类
    /// </summary>
    /// <param name="category">分类信息</param>
    /// <returns></returns>
    Task<ApiResponse<CategoryVO>> AddCategoryAsync(EditCategoryDTO category);

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="category">分类信息</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> UpdateCategoryAsync(int id, EditCategoryDTO category);

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);

    /// <summary>
    /// 批量删除分类
    /// </summary>
    /// <param name="ids">分类ID列表</param>
    /// <returns></returns>
    Task<ApiResponse<bool>> DeleteCategoriesAsync(List<int> ids);
}