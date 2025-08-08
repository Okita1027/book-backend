namespace book_backend.utils;

public class Pagination<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public required List<T> Items { get; set; }
}