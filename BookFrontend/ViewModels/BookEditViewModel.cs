using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services;
using book_frontend.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation;

namespace book_frontend.ViewModels;

public partial class BookEditViewModel : ObservableObject
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly IPublisherService _publisherService;
    private readonly ICategoryService _categoryService;
    private readonly BookEditValidator _validator;
    private readonly LoggingService _loggingService;

    // 基本属性
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _isbn = string.Empty;
    
    [ObservableProperty]
    private DateTime _publishedDate = DateTime.Now;
    
    [ObservableProperty]
    private int _stock = 0;
    
    [ObservableProperty]
    private int _available = 0;
    
    [ObservableProperty]
    private int? _selectedAuthorId;
    
    [ObservableProperty]
    private int? _selectedPublisherId;
    
    [ObservableProperty]
    private ObservableCollection<int> _selectedCategoryIds = new();

    // 下拉数据源
    [ObservableProperty]
    private ObservableCollection<AuthorVO> _authors = new();
    
    [ObservableProperty]
    private ObservableCollection<PublisherVO> _publishers = new();
    
    [ObservableProperty]
    private ObservableCollection<CategoryVO> _categories = new();

    // UI状态
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private bool _isSaving = false;
    
    [ObservableProperty]
    private string _validationErrors = string.Empty;
    
    [ObservableProperty]
    private bool _isEditMode = false;
    
    private int? _bookId;

    public BookEditViewModel(
        IBookService bookService,
        IAuthorService authorService,
        IPublisherService publisherService,
        ICategoryService categoryService,
        LoggingService loggingService)
    {
        _bookService = bookService;
        _authorService = authorService;
        _publisherService = publisherService;
        _categoryService = categoryService;
        _validator = new BookEditValidator();
        _loggingService = loggingService;

        InitializeCommands();
    }

    #region 属性

    // 属性将由 [ObservableProperty] 特性自动生成
    // Title, Isbn, PublishedDate, Stock, Available, SelectedAuthorId, SelectedPublisherId, SelectedCategoryIds
    // Authors, Publishers, Categories, IsLoading, IsSaving, ValidationErrors, IsEditMode
    
    // Stock属性变化时的业务逻辑
    partial void OnStockChanged(int value)
    {
        // 库存变化时，可用数量不能超过总库存
        if (Available > value)
        {
            Available = value;
        }
    }

    public string ValidationErrorMessage => ValidationErrors;

    public bool HasValidationErrors => !string.IsNullOrEmpty(ValidationErrors);

    public string WindowTitle => IsEditMode ? "编辑图书" : "新增图书";

    #endregion

    #region 命令

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }

    #endregion

    #region 命令初始化

    private void InitializeCommands()
    {
        SaveCommand = new RelayCommand(async () => await SaveBookAsync(), () => !IsSaving);
        CancelCommand = new RelayCommand(() => CloseDialog(false));
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 初始化对话框数据
    /// </summary>
    public async Task InitializeAsync(int? bookId = null)
    {
        try
        {
            IsLoading = true;
            _bookId = bookId;
            IsEditMode = bookId.HasValue;

            // 并行加载下拉数据
            var loadTasks = new List<Task>
            {
                LoadAuthorsAsync(),
                LoadPublishersAsync(),
                LoadCategoriesAsync()
            };

            await Task.WhenAll(loadTasks);

            // 如果是编辑模式，加载图书数据
            if (IsEditMode && bookId.HasValue)
            {
                await LoadBookDataAsync(bookId.Value);
            }
            else
            {
                // 新增模式，设置默认值
                Available = Stock;
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogErrorAndShowMessage(ex, "初始化图书编辑对话框数据失败，请重试。", "BookEditViewModel.InitializeAsync");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 加载作者数据
    /// </summary>
    private async Task LoadAuthorsAsync()
    {
        var response = await _authorService.GetAllAuthorsAsync();
        if (response is { Success: true, Data: not null })
        {
            Authors = new ObservableCollection<AuthorVO>(response.Data);
        }
    }

    /// <summary>
    /// 加载出版社数据
    /// </summary>
    private async Task LoadPublishersAsync()
    {
        var response = await _publisherService.GetAllPublishersAsync();
        if (response is { Success: true, Data: not null })
        {
            Publishers = new ObservableCollection<PublisherVO>(response.Data);
        }
    }

    /// <summary>
    /// 加载分类数据
    /// </summary>
    private async Task LoadCategoriesAsync()
    {
        var response = await _categoryService.GetAllCategoriesAsync();
        if (response is { Success: true, Data: not null })
        {
            Categories = new ObservableCollection<CategoryVO>(response.Data);

            // 为每个分类添加选择状态变化监听
            foreach (var category in Categories)
            {
                category.PropertyChanged += Category_PropertyChanged;
            }
        }
    }

    /// <summary>
    /// 分类选择状态变化处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Category_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CategoryVO.IsSelected) && sender is CategoryVO category)
        {
            if (category.IsSelected && !SelectedCategoryIds.Contains(category.Id))
            {
                SelectedCategoryIds.Add(category.Id);
            }
            else if (!category.IsSelected && SelectedCategoryIds.Contains(category.Id))
            {
                SelectedCategoryIds.Remove(category.Id);
            }
        }
    }

    /// <summary>
    /// 加载图书数据（编辑模式）
    /// </summary>
    private async Task LoadBookDataAsync(int bookId)
    {
        try
        {
            var response = await _bookService.GetBookByIdAsync(bookId);
            if (response is { Success: true, Data: not null })
            {
                var book = response.Data;
                Title = book.Title ?? string.Empty;
                Isbn = book.Isbn ?? string.Empty;
                PublishedDate = book.PublishedDate;
                Stock = book.Stock;
                Available = book.Available;
                SelectedAuthorId = book.AuthorId;
                SelectedPublisherId = book.PublisherId;

                // 设置选中的分类
                var selectedCategoryIds = book.CategoryIds ?? new List<int>();
                SelectedCategoryIds = new ObservableCollection<int>(selectedCategoryIds);

                // 更新分类的选中状态
                foreach (var category in Categories)
                {
                    category.IsSelected = selectedCategoryIds.Contains(category.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogErrorAndShowMessage(ex, "加载图书数据失败，请重试。", "BookEditViewModel.LoadBookDataAsync");
        }
    }

    /// <summary>
    /// 保存图书
    /// </summary>
    private async Task SaveBookAsync()
    {
        try
        {
            // 验证数据
            if (!ValidateData())
            {
                return;
            }

            IsSaving = true;

            var bookDto = new EditBookDTO
            {
                Title = Title.Trim(),
                Isbn = Isbn.Trim(),
                PublishedDate = PublishedDate,
                Stock = Stock,
                Available = Available,
                AuthorId = SelectedAuthorId!.Value,
                PublisherId = SelectedPublisherId!.Value,
                CategoryIds = SelectedCategoryIds.ToList()
            };

            ApiResponse<BookVO> response;
            if (IsEditMode && _bookId.HasValue)
            {
                response = await _bookService.UpdateBookAsync(_bookId.Value, bookDto);
            }
            else
            {
                response = await _bookService.AddBookAsync(bookDto);
            }

            if (response.Success)
            {
                _loggingService.LogSuccessMessage(IsEditMode ? "图书更新成功！" : "图书添加成功！", "成功");
                CloseDialog(true);
            }
            else
            {
                _loggingService.LogApiError(response.Message, "BookEditViewModel.SaveBookAsync",
                    IsEditMode ? "更新图书" : "添加图书");
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogErrorAndShowMessage(ex, "保存图书时发生异常，请重试。", "BookEditViewModel.SaveBookAsync");
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// 验证数据
    /// </summary>
    private bool ValidateData()
    {
        var dto = new EditBookDTO
        {
            Title = Title?.Trim() ?? string.Empty,
            Isbn = Isbn?.Trim() ?? string.Empty,
            PublishedDate = PublishedDate,
            Stock = Stock,
            Available = Available,
            AuthorId = SelectedAuthorId ?? 0,
            PublisherId = SelectedPublisherId ?? 0,
            CategoryIds = SelectedCategoryIds?.ToList() ?? new List<int>()
        };

        var result = _validator.Validate(dto);
        if (!result.IsValid)
        {
            ValidationErrors = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
            OnPropertyChanged(nameof(ValidationErrorMessage));
            OnPropertyChanged(nameof(HasValidationErrors));
            return false;
        }

        ValidationErrors = string.Empty;
        OnPropertyChanged(nameof(ValidationErrorMessage));
        OnPropertyChanged(nameof(HasValidationErrors));
        return true;
    }

    /// <summary>
    /// 关闭对话框
    /// </summary>
    private void CloseDialog(bool dialogResult)
    {
        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.DialogResult = dialogResult;
            window.Close();
        }
    }

    #endregion


}

/// <summary>
/// 图书编辑验证器
/// </summary>
public class BookEditValidator : AbstractValidator<EditBookDTO>
{
    public BookEditValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("图书标题不能为空")
            .MaximumLength(200).WithMessage("图书标题不能超过200个字符");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN不能为空")
            .Must(isbn => isbn.Length is 10 or 13).WithMessage("ISBN只能是10位或13位")
            .Matches(@"^[0-9]+$").WithMessage("ISBN只能包含数字");

        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("出版日期不能晚于今天")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("出版日期不能早于1900年");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("库存数量不能为负数")
            .LessThanOrEqualTo(1000).WithMessage("库存数量不能超过1000");

        RuleFor(x => x.Available)
            .GreaterThanOrEqualTo(0).WithMessage("可用数量不能为负数")
            .LessThanOrEqualTo(x => x.Stock).WithMessage("可用数量不能超过库存数量");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("请选择作者");

        RuleFor(x => x.PublisherId)
            .GreaterThan(0).WithMessage("请选择出版社");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("请至少选择一个分类")
            .Must(ids => ids.All(id => id > 0)).WithMessage("分类ID无效");
    }
}