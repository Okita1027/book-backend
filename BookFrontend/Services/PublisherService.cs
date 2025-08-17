using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.Services;

public class PublisherService : IPublisherService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(ApiClient apiClient, ILogger<PublisherService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponse<List<PublisherVO>>> GetAllPublishersAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<List<PublisherVO>>("/api/Publishers");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting publishers");
            return new ApiResponse<List<PublisherVO>> { Success = false, Message = "获取出版社列表失败" };
        }
    }

    public async Task<ApiResponse<PublisherVO>> GetPublisherByIdAsync(int id)
    {
        try
        {
            var response = await _apiClient.GetAsync<PublisherVO>($"/api/Publishers/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting publisher {Id}", id);
            return new ApiResponse<PublisherVO> { Success = false, Message = "获取出版社信息失败" };
        }
    }

    public async Task<ApiResponse<PublisherVO>> AddPublisherAsync(EditPublisherDTO publisher)
    {
        try
        {
            var response = await _apiClient.PostAsync<PublisherVO>("/api/Publishers", publisher);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding publisher");
            return new ApiResponse<PublisherVO> { Success = false, Message = "添加出版社失败" };
        }
    }

    public async Task<ApiResponse<bool>> UpdatePublisherAsync(int id, EditPublisherDTO publisher)
    {
        try
        {
            var response = await _apiClient.PutAsync<bool>($"/api/Publishers/{id}", publisher);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating publisher {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "更新出版社失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeletePublisherAsync(int id)
    {
        try
        {
            var response = await _apiClient.DeleteAsync($"/api/Publishers/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting publisher {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "删除出版社失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeletePublishersAsync(List<int> ids)
    {
        try
        {
            var response = await _apiClient.DeleteAsync("/api/Publishers", ids);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting publishers");
            return new ApiResponse<bool> { Success = false, Message = "批量删除出版社失败" };
        }
    }
}