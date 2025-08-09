using System.Windows.Documents;

namespace book_frontend.Models;

/// <summary>
/// 通用API响应模型
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public int Code { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
}

public class PagedResponse<T>
{
    public int Total { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    private int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    public bool HasNextPage => PageIndex < TotalPages;
    public bool HasPreviousPage => PageIndex > 1;
    public List<T> Items { get; set; } = [];
}