using book_frontend.Helpers;
using book_frontend.Models;
using book_frontend.Services.Interfaces;

namespace book_frontend.Services;

public class BookService : IBookService
{
    private readonly ApiClient _apiClient;

    public BookService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<List<Book>>> GetAllBooksAsync()
    {
        return await _apiClient.GetAsync<List<Book>>("Books");
    }

    public async Task<ApiResponse<Book>> GetBookByIdAsync(int id)
    {
        return await _apiClient.GetAsync<Book>($"Books/{id}");
    }

    public async Task<ApiResponse<PagedResponse<Book>>> SearchBooksAsync(
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
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(title))
            queryParams.Add($"title={Uri.EscapeDataString(title)}");

        queryParams.Add($"pageIndex={pageIndex}");
        queryParams.Add($"pageSize={pageSize}");

        var queryString = string.Join("&", queryParams);
        var endpoint = $"Books/searchPaginated?{queryString}";

        return await _apiClient.GetAsync<PagedResponse<Book>>(endpoint);
    }

    public async Task<ApiResponse<Book>> AddBookAsync(Book book)
    {
        return await _apiClient.PostAsync<Book>("Books/add", book);
    }

    public async Task<ApiResponse<Book>> UpdateBookAsync(int id, Book book)
    {
        return await _apiClient.PutAsync<Book>($"Books/{id}", book);
    }

    public async Task<ApiResponse<bool>> DeleteBookAsync(int id)
    {
        return await _apiClient.DeleteAsync($"Books/{id}");
    }
}