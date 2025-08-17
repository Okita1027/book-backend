using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.Services;

public class AuthorService : IAuthorService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(ApiClient apiClient, ILogger<AuthorService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponse<List<AuthorVO>>> GetAllAuthorsAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<List<AuthorVO>>("/api/Authors");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authors");
            return new ApiResponse<List<AuthorVO>> { Success = false, Message = "获取作者列表失败" };
        }
    }

    public async Task<ApiResponse<AuthorVO>> GetAuthorByIdAsync(int id)
    {
        try
        {
            var response = await _apiClient.GetAsync<AuthorVO>($"/api/Authors/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting author {Id}", id);
            return new ApiResponse<AuthorVO> { Success = false, Message = "获取作者信息失败" };
        }
    }

    public async Task<ApiResponse<List<BookVO>>> GetAuthorBooksAsync(int id)
    {
        try
        {
            var response = await _apiClient.GetAsync<List<BookVO>>($"/api/Authors/{id}/books");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting author books {Id}", id);
            return new ApiResponse<List<BookVO>> { Success = false, Message = "获取作者图书列表失败" };
        }
    }

    public async Task<ApiResponse<AuthorVO>> AddAuthorAsync(EditAuthorDTO author)
    {
        try
        {
            var response = await _apiClient.PostAsync<AuthorVO>("/api/Authors", author);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding author");
            return new ApiResponse<AuthorVO> { Success = false, Message = "添加作者失败" };
        }
    }

    public async Task<ApiResponse<bool>> UpdateAuthorAsync(int id, EditAuthorDTO author)
    {
        try
        {
            var response = await _apiClient.PutAsync<bool>($"/api/Authors/{id}", author);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "更新作者失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteAuthorAsync(int id)
    {
        try
        {
            var response = await _apiClient.DeleteAsync($"/api/Authors/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "删除作者失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteAuthorsAsync(List<int> ids)
    {
        try
        {
            var response = await _apiClient.DeleteAsync("/api/Authors", ids);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting authors");
            return new ApiResponse<bool> { Success = false, Message = "批量删除作者失败" };
        }
    }
}