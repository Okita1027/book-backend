using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.Services;

public class CategoryService : ICategoryService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApiClient apiClient, ILogger<CategoryService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponse<List<CategoryVO>>> GetAllCategoriesAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<List<CategoryVO>>("Categories");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new ApiResponse<List<CategoryVO>> { Success = false, Message = "获取分类列表失败" };
        }
    }

    public async Task<ApiResponse<CategoryVO>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var response = await _apiClient.GetAsync<CategoryVO>($"Categories/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {Id}", id);
            return new ApiResponse<CategoryVO> { Success = false, Message = "获取分类信息失败" };
        }
    }

    public async Task<ApiResponse<CategoryVO>> AddCategoryAsync(EditCategoryDTO category)
    {
        try
        {
            var response = await _apiClient.PostAsync<CategoryVO>("Categories", category);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category");
            return new ApiResponse<CategoryVO> { Success = false, Message = "添加分类失败" };
        }
    }

    public async Task<ApiResponse<bool>> UpdateCategoryAsync(int id, EditCategoryDTO category)
    {
        try
        {
            var response = await _apiClient.PutAsync<bool>($"Categories/{id}", category);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "更新分类失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await _apiClient.DeleteAsync($"Categories/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "删除分类失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteCategoriesAsync(List<int> ids)
    {
        try
        {
            var response = await _apiClient.DeleteAsync("Categories", ids);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting categories");
            return new ApiResponse<bool> { Success = false, Message = "批量删除分类失败" };
        }
    }
}