using book_frontend.Services.Interfaces;

namespace book_frontend.ViewModels;

public class MainViewModel : BaseViewModel
{
    public BookListViewModel Home { get; }

    public MainViewModel(IBookService bookService)
    {
        Home = new BookListViewModel(bookService);
    }
}