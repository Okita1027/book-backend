namespace book_frontend.Models.VOs;

public class AuthorVO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
}