using System.Net.Http;
using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.Services;

public class BookService : IBookService
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<BookService> _logger;

    public BookService(ApiClient apiClient, ILogger<BookService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponse<List<BookVO>>> GetAllBooksAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<List<BookVO>>("Books");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all books");
            return new ApiResponse<List<BookVO>> { Success = false, Message = "获取图书列表失败" };
        }
    }

    public async Task<ApiResponse<BookVO>> GetBookByIdAsync(int id)
    {
        try
        {
            var response = await _apiClient.GetAsync<BookVO>($"Books/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting book {Id}", id);
            return new ApiResponse<BookVO> { Success = false, Message = "获取图书详情失败" };
        }
    }

    public async Task<ApiResponse<PagedResponse<BookVO>>> SearchBooksAsync(
        string? title = null,
        string? author = null, 
        string? category = null,
        string? publisher = null,
        string? isbn = null,
        DateTime? publishDateStart = null,
        DateTime? publishDateEnd = null,
        int pageIndex = 1, 
        int pageSize = 12
    )
    {
        try
        {
            var queryParams = new List<string>
            {
                // 添加分页参数
                $"pageIndex={pageIndex}",
                $"pageSize={pageSize}"
            };

            // 添加搜索条件
            if (!string.IsNullOrEmpty(title))
                queryParams.Add($"title={Uri.EscapeDataString(title)}");
            if (!string.IsNullOrEmpty(author))
                queryParams.Add($"authorName={Uri.EscapeDataString(author)}");
            if (!string.IsNullOrEmpty(category))
                queryParams.Add($"categoryName={Uri.EscapeDataString(category)}");
            if (!string.IsNullOrEmpty(publisher))
                queryParams.Add($"publisherName={Uri.EscapeDataString(publisher)}");
            if (!string.IsNullOrEmpty(isbn))
                queryParams.Add($"isbn={Uri.EscapeDataString(isbn)}");
            if (publishDateStart.HasValue)
                queryParams.Add($"publishedDateBegin={publishDateStart.Value:yyyy-MM-dd}");
            if (publishDateEnd.HasValue)
                queryParams.Add($"publishedDateEnd={publishDateEnd.Value:yyyy-MM-dd}");

            var queryString = string.Join("&", queryParams);
            var endpoint = $"Books/searchPaginated?{queryString}";

            var response = await _apiClient.GetAsync<PagedResponse<BookVO>>(endpoint);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books with parameters: title={Title}, author={Author}, category={Category}, publisher={Publisher}, isbn={Isbn}", 
                title, author, category, publisher, isbn);
            return new ApiResponse<PagedResponse<BookVO>> { Success = false, Message = "搜索图书失败" };
        }
    }

    public async Task<ApiResponse<BookVO>> AddBookAsync(EditBookDTO book)
    {
        try
        {
            var response = await _apiClient.PostAsync<BookVO>("Books/add", book);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book {Title}", book.Title);
            return new ApiResponse<BookVO> { Success = false, Message = "添加图书失败" };
        }
    }

    public async Task<ApiResponse<BookVO>> UpdateBookAsync(int id, EditBookDTO book)
    {
        try
        {
            var response = await _apiClient.PutAsync<BookVO>($"Books/{id}", book);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book {Id}", id);
            return new ApiResponse<BookVO> { Success = false, Message = "更新图书失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteBookAsync(int id)
    {
        try
        {
            var response = await _apiClient.DeleteAsync($"Books/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book {Id}", id);
            return new ApiResponse<bool> { Success = false, Message = "删除图书失败" };
        }
    }

    public async Task<ApiResponse<bool>> DeleteBooksAsync(List<int> ids)
    {
        try
        {
            var response = await _apiClient.DeleteAsync("Books", ids);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting books");
            return new ApiResponse<bool> { Success = false, Message = "批量删除图书失败" };
        }
    }
}