using System.Net.Http;
using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;

namespace book_frontend.Services;

public class BookService : IBookService
{
    private readonly ApiClient _apiClient;

    public BookService()
    {
        var httpClient = new HttpClient();
        
        // 从配置文件读取API基础地址
        var config = ConfigurationHelper.GetConfig();
        var apiBaseUrl = !string.IsNullOrEmpty(config.ApiBaseUrl) ? config.ApiBaseUrl : "http://localhost:8888/api/";
        
        httpClient.BaseAddress = new Uri(apiBaseUrl);
        _apiClient = new ApiClient(httpClient);
    }

    public BookService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<List<BookVO>>> GetAllBooksAsync()
    {
        return await _apiClient.GetAsync<List<BookVO>>("Books");
    }

    public async Task<ApiResponse<BookVO>> GetBookByIdAsync(int id)
    {
        return await _apiClient.GetAsync<BookVO>($"Books/{id}");
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

        return await _apiClient.GetAsync<PagedResponse<BookVO>>(endpoint);
    }

    public async Task<ApiResponse<BookVO>> AddBookAsync(EditBookDTO book)
    {
        return await _apiClient.PostAsync<BookVO>("Books/add", book);
    }

    public async Task<ApiResponse<BookVO>> UpdateBookAsync(int id, EditBookDTO book)
    {
        return await _apiClient.PutAsync<BookVO>($"Books/{id}", book);
    }

    public async Task<ApiResponse<bool>> DeleteBookAsync(int id)
    {
        return await _apiClient.DeleteAsync($"Books/{id}");
    }
}