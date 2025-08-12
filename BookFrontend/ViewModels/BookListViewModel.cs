using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using book_frontend.Commands;
using book_frontend.Models;
using book_frontend.Services.Interfaces;

namespace book_frontend.ViewModels;

public class BookListViewModel : BaseViewModel
{
    private readonly IBookService _bookService;

    public BookListViewModel(IBookService bookService)
    {
        _bookService = bookService;
        Books = new ObservableCollection<Book>();
        
        // 先初始化所有命令
        SearchCommand = new RelayCommand(async _ => await SearchAsync(), _ => !IsLoading);
        ResetCommand = new RelayCommand(_ => Reset(), _ => !IsLoading);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(PageIndex + 1), _ => !IsLoading && HasNextPage);
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(PageIndex - 1), _ => !IsLoading && HasPreviousPage);
        
        // 然后设置属性值，避免在命令初始化前调用RefreshCommands
        PageSize = 12;
        PageIndex = 1;
        
        // 初始化时加载数据
        _ = InitializeAsync();
    }
    
    /// <summary>
    /// 初始化加载数据
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadPageAsync(1);
    }

    // 搜索条件
    private string? _title;
    public string? Title { get => _title; set { if (SetProperty(ref _title, value)) RefreshCommands(); } }

    private string? _author;
    public string? Author { get => _author; set { if (SetProperty(ref _author, value)) RefreshCommands(); } }

    private string? _category;
    public string? Category { get => _category; set { if (SetProperty(ref _category, value)) RefreshCommands(); } }

    private string? _publisher;
    public string? Publisher { get => _publisher; set { if (SetProperty(ref _publisher, value)) RefreshCommands(); } }

    private string? _isbn;
    public string? Isbn { get => _isbn; set { if (SetProperty(ref _isbn, value)) RefreshCommands(); } }

    private DateTime? _publishDateStart;
    public DateTime? PublishDateStart { get => _publishDateStart; set { if (SetProperty(ref _publishDateStart, value)) RefreshCommands(); } }

    private DateTime? _publishDateEnd;
    public DateTime? PublishDateEnd { get => _publishDateEnd; set { if (SetProperty(ref _publishDateEnd, value)) RefreshCommands(); } }

    // 分页
    private int _pageIndex;
    public int PageIndex { get => _pageIndex; set { if (SetProperty(ref _pageIndex, value)) RefreshCommands(); } }

    private int _pageSize;
    public int PageSize { get => _pageSize; set { if (SetProperty(ref _pageSize, value)) RefreshCommands(); } }

    private int _total;
    public int Total { get => _total; set { if (SetProperty(ref _total, value)) RefreshCommands(); } }

    public bool HasNextPage => PageIndex * PageSize < Total;
    public bool HasPreviousPage => PageIndex > 1;

    // 列表和状态
    public ObservableCollection<Book> Books { get; }

    private bool _isLoading;
    public bool IsLoading { get => _isLoading; set { if (SetProperty(ref _isLoading, value)) RefreshCommands(); } }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    // 命令
    public RelayCommand SearchCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand PrevPageCommand { get; }

    private async Task SearchAsync()
    {
        await LoadPageAsync(1);
    }

    private void Reset()
    {
        Title = Author = Category = Publisher = Isbn = null;
        PublishDateStart = PublishDateEnd = null;
        PageIndex = 1;
        Books.Clear();
        Total = 0;
        ErrorMessage = null;
        
        // 重置后重新加载数据
        _ = LoadPageAsync(1);
    }

    private async Task GoToPageAsync(int pageIndex)
    {
        if (pageIndex < 1) return;
        await LoadPageAsync(pageIndex);
    }

    private async Task LoadPageAsync(int pageIndex)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            var resp = await _bookService.SearchBooksAsync(
                title: Title,
                author: Author,
                category: Category,
                publisher: Publisher,
                isbn: Isbn,
                publishDateStart: PublishDateStart,
                publishDateEnd: PublishDateEnd,
                pageIndex: pageIndex,
                pageSize: PageSize);

            Books.Clear();
            if (resp.Success && resp.Data != null)
            {
                foreach (var b in resp.Data.Items)
                    Books.Add(b);
                PageIndex = resp.Data.PageIndex;
                PageSize = resp.Data.PageSize;
                Total = resp.Data.Total;
            }
            else
            {
                ErrorMessage = resp.Message ?? "查询失败";
                Total = 0;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void RefreshCommands()
    {
        SearchCommand.RaiseCanExecuteChanged();
        ResetCommand?.RaiseCanExecuteChanged();
        NextPageCommand?.RaiseCanExecuteChanged();
        PrevPageCommand?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));
    }
}