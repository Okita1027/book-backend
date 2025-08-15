using System.Collections.ObjectModel;
using book_frontend.Models;
using book_frontend.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace book_frontend.ViewModels;

public class BookListViewModel : BaseViewModel
{
    private readonly IBookService _bookService;
    private readonly ILogger _logger;

    // 搜索条件
    private string? _title;

    public string? Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value)) RefreshCommands();
        }
    }

    private string? _author;

    public string? Author
    {
        get => _author;
        set
        {
            if (SetProperty(ref _author, value)) RefreshCommands();
        }
    }

    private string? _category;

    public string? Category
    {
        get => _category;
        set
        {
            if (SetProperty(ref _category, value)) RefreshCommands();
        }
    }

    private string? _publisher;

    public string? Publisher
    {
        get => _publisher;
        set
        {
            if (SetProperty(ref _publisher, value)) RefreshCommands();
        }
    }

    private string? _isbn;

    public string? Isbn
    {
        get => _isbn;
        set
        {
            if (SetProperty(ref _isbn, value)) RefreshCommands();
        }
    }

    private DateTime? _publishDateStart;

    public DateTime? PublishDateStart
    {
        get => _publishDateStart;
        set
        {
            if (SetProperty(ref _publishDateStart, value)) RefreshCommands();
        }
    }

    private DateTime? _publishDateEnd;

    public DateTime? PublishDateEnd
    {
        get => _publishDateEnd;
        set
        {
            if (SetProperty(ref _publishDateEnd, value)) RefreshCommands();
        }
    }

    // 分页
    private int _pageIndex;

    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            if (SetProperty(ref _pageIndex, value)) RefreshCommands();
        }
    }

    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value)) RefreshCommands();
        }
    }

    private int _total;

    public int Total
    {
        get => _total;
        set
        {
            if (SetProperty(ref _total, value)) RefreshCommands();
        }
    }

    public bool HasNextPage => PageIndex * PageSize < Total;
    public bool HasPreviousPage => PageIndex > 1;

    /**
     * 列表和状态
     * ObservableCollection：当集合中的元素被添加、删除或更新时，它会自动通知用户界面（UI）进行相应的更新
     */
    public ObservableCollection<Book> Books { get; }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value)) RefreshCommands();
        }
    }

    private string? _errorMessage;

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // 页码跳转
    private string _jumpToPageInput = "1";

    public string JumpToPageInput
    {
        get => _jumpToPageInput;
        set
        {
            if (SetProperty(ref _jumpToPageInput, value))
            {
                RefreshCommands();
            }
        }
    }

    public int TotalPages => Total > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 1;

    // 命令
    public RelayCommand SearchCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand PrevPageCommand { get; }
    public RelayCommand JumpToPageCommand { get; }

    public BookListViewModel(IBookService bookService)
    {
        _bookService = bookService;
        _logger = Log.ForContext<BookListViewModel>();
        Books = [];

        // 先初始化所有命令
        SearchCommand = new RelayCommand(async () => await SearchAsync(), () => !IsLoading);
        ResetCommand = new RelayCommand(Reset, () => !IsLoading);
        NextPageCommand = new RelayCommand(async () => await GoToPageAsync(PageIndex + 1),
            () => !IsLoading && HasNextPage);
        PrevPageCommand = new RelayCommand(async () => await GoToPageAsync(PageIndex - 1),
            () => !IsLoading && HasPreviousPage);
        JumpToPageCommand =
            new RelayCommand(async () => await JumpToPageAsync(), () => !IsLoading && CanJumpToPage());

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
        if (pageIndex < 1)
        {
            return;
        }

        await LoadPageAsync(pageIndex);
    }

    private async Task LoadPageAsync(int pageIndex, bool append = false)
    {
        _logger.Information(
            "开始搜索图书，搜索条件: title={Title}, author={Author}, category={Category}, publisher={Publisher}, isbn={Isbn}, publishDateStart={PublishDateStart}, publishDateEnd={PublishDateEnd}, pageIndex={PageIndex}, pageSize={PageSize}",
            Title, Author, Category, Publisher, Isbn, PublishDateStart, PublishDateEnd, pageIndex, PageSize);

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            var response = await _bookService.SearchBooksAsync(
                title: Title,
                author: Author,
                category: Category,
                publisher: Publisher,
                isbn: Isbn,
                publishDateStart: PublishDateStart,
                publishDateEnd: PublishDateEnd,
                pageIndex: pageIndex,
                pageSize: PageSize);
            // 更新 UI 数据
            if (!append)
            {
                Books.Clear();
            }

            if (response is { Success: true, Data: not null })
            {
                foreach (var b in response.Data.Items)
                {
                    Books.Add(b);
                }

                PageIndex = response.Data.PageIndex;
                PageSize = response.Data.PageSize;
                Total = response.Data.Total;
                _logger.Information("搜索完成，找到 {Count} 本书籍", response.Data.Items.Count);
                // 同步跳转输入框显示为当前页
                JumpToPageInput = PageIndex.ToString();
            }
            else
            {
                ErrorMessage = response.Message ?? "查询失败";
                if (!append)
                {
                    Total = 0;
                    _logger.Warning("搜索失败：{Message}", response.Message);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.Error(ex, "搜索图书时发生异常");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadNextPageAsync()
    {
        if (IsLoading) return;
        if (!HasNextPage) return;
        var next = PageIndex + 1;
        await LoadPageAsync(next, append: true);
    }

    private void RefreshCommands()
    {
        SearchCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
        JumpToPageCommand.NotifyCanExecuteChanged();
        /*
         * HasNextPage 和 HasPreviousPage是计算属性
         * 它们的值依赖于其他属性（PageIndex、PageSize、Total）的变化
         * 当执行分页操作时，这些依赖属性会改变，但 HasNextPage 和 HasPreviousPage 本身不会自动通知UI更新
         * 所以需要手动调用 OnPropertyChanged 来通知UI这些计算属性的值已经改变
         */
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));
        OnPropertyChanged(nameof(TotalPages));
    }
// removed erroneous class closing brace here to keep following methods inside the class

    private bool CanJumpToPage()
    {
        if (string.IsNullOrWhiteSpace(JumpToPageInput)) return false;
        if (!int.TryParse(JumpToPageInput, out var page)) return false;
        if (page < 1) return false;
        // 如果总条数为0，允许跳到第1页以触发查询
        var totalPages = TotalPages;
        return page <= totalPages;
    }

    private async Task JumpToPageAsync()
    {
        if (!int.TryParse(JumpToPageInput, out var page)) return;
        if (page < 1) page = 1;
        var totalPages = TotalPages;
        if (page > totalPages) page = totalPages;
        await GoToPageAsync(page);
    }
}