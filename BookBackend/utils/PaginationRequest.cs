namespace book_backend.utils;

public class PaginationRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } // "ascend" or "descend"
}