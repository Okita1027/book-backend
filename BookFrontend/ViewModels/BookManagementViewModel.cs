using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using book_frontend.Models.DTOs;
using book_frontend.Models.VOs;
using book_frontend.Services.Interfaces;
using book_frontend.Services;
using CommunityToolkit.Mvvm.Input;

namespace book_frontend.ViewModels
{
    public class BookManagementViewModel : INotifyPropertyChanged
    {
        private readonly IBookService _bookService;
        private readonly LoggingService _loggingService;

        // 搜索条件
        private string _searchTitle = string.Empty;
        private string _searchAuthor = string.Empty;
        private string _searchIsbn = string.Empty;
        private string _searchPublisher = string.Empty;
        private string _searchCategory = string.Empty;

        // 分页相关
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalCount = 0;
        private int _totalPages = 0;
        private int _jumpToPage = 1;

        // 数据集合
        private ObservableCollection<BookVOWrapper> _books = new();
        private ObservableCollection<BookVOWrapper> _selectedBooks = new();

        // 加载状态
        private bool _isLoading = false;

        public BookManagementViewModel(IBookService bookService, LoggingService loggingService)
        {
            _bookService = bookService;
            _loggingService = loggingService;
            InitializeCommands();

            // 初始加载数据
            _ = LoadBooksAsync();
        }

        #region 属性

        public string SearchTitle
        {
            get => _searchTitle;
            set => SetProperty(ref _searchTitle, value);
        }

        public string SearchAuthor
        {
            get => _searchAuthor;
            set => SetProperty(ref _searchAuthor, value);
        }

        public string SearchIsbn
        {
            get => _searchIsbn;
            set => SetProperty(ref _searchIsbn, value);
        }

        public string SearchPublisher
        {
            get => _searchPublisher;
            set => SetProperty(ref _searchPublisher, value);
        }

        public string SearchCategory
        {
            get => _searchCategory;
            set => SetProperty(ref _searchCategory, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public int JumpToPage
        {
            get => _jumpToPage;
            set => SetProperty(ref _jumpToPage, value);
        }

        public ObservableCollection<BookVOWrapper> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ObservableCollection<BookVOWrapper> SelectedBooks
        {
            get => _selectedBooks;
            set => SetProperty(ref _selectedBooks, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region 命令

        public ICommand SearchCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand FirstPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand LastPageCommand { get; private set; }
        public ICommand JumpToPageCommand { get; private set; }

        #endregion

        #region 命令初始化

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(() => SearchBooksAsync());
            ResetCommand = new RelayCommand(() => ResetSearchAsync());
            AddCommand = new RelayCommand(() => AddBook());
            EditCommand = new RelayCommand(() => EditBook());
            DeleteCommand = new RelayCommand(() => DeleteBooks());
            FirstPageCommand = new RelayCommand(() => GoToFirstPageAsync());
            PreviousPageCommand = new RelayCommand(() => GoToPreviousPageAsync());
            NextPageCommand = new RelayCommand(() => GoToNextPageAsync());
            LastPageCommand = new RelayCommand(() => GoToLastPageAsync());
            JumpToPageCommand = new RelayCommand(() => JumpToSpecificPageAsync());
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 加载图书数据
        /// </summary>
        private async Task LoadBooksAsync()
        {
            try
            {
                IsLoading = true;

                var response = await _bookService.SearchBooksAsync(
                    SearchTitle,
                    SearchAuthor,
                    SearchCategory,
                    SearchPublisher,
                    SearchIsbn,
                    null, // publishDateStart
                    null, // publishDateEnd
                    CurrentPage,
                    PageSize
                );

                if (response.Success && response.Data != null)
                {
                    var bookWrappers = response.Data.Items.Select(book => new BookVOWrapper(book)).ToList();
                    Books = new ObservableCollection<BookVOWrapper>(bookWrappers);

                    TotalCount = response.Data.Total;
                    TotalPages = (int)Math.Ceiling((double)response.Data.Total / response.Data.PageSize);

                    // 清空选中项
                    SelectedBooks.Clear();
                }
                else
                {
                    _loggingService.LogApiError(response.Message, "BookManagementViewModel.LoadBooksAsync", "加载图书数据");
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorAndShowMessage(ex, "加载图书数据时发生异常，请重试。", "BookManagementViewModel.LoadBooksAsync");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 搜索图书
        /// </summary>
        private async Task SearchBooksAsync()
        {
            CurrentPage = 1; // 重置到第一页
            await LoadBooksAsync();
        }

        /// <summary>
        /// 重置搜索条件
        /// </summary>
        private async Task ResetSearchAsync()
        {
            SearchTitle = string.Empty;
            SearchAuthor = string.Empty;
            SearchIsbn = string.Empty;
            SearchPublisher = string.Empty;
            SearchCategory = string.Empty;
            CurrentPage = 1;

            await LoadBooksAsync();
        }

        /// <summary>
        /// 新增图书
        /// </summary>
        private async void AddBook()
        {
            try
            {
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (currentWindow == null)
                {
                    currentWindow = Application.Current.MainWindow;
                }

                var result = await Views.BookEditDialog.ShowAddDialogAsync(currentWindow);
                if (result == true)
                {
                    // 刷新图书列表
                    await LoadBooksAsync();
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorAndShowMessage(ex, "打开新增图书对话框时发生错误，请重试。", "BookManagementViewModel.AddBook");
            }
        }

        /// <summary>
        /// 编辑图书
        /// </summary>
        private async void EditBook()
        {
            var selectedItems = Books.Where(b => b.IsSelected).ToList();
            if (selectedItems.Count == 0)
            {
                _loggingService.LogWarningMessage("请选择要编辑的图书", "提示");
                return;
            }

            if (selectedItems.Count > 1)
            {
                _loggingService.LogWarningMessage("只能选择一本图书进行编辑", "提示");
                return;
            }

            try
            {
                var selectedBook = selectedItems.First();
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (currentWindow == null)
                {
                    currentWindow = Application.Current.MainWindow;
                }

                var result = await Views.BookEditDialog.ShowEditDialogAsync(currentWindow, selectedBook.Book.Id);
                if (result == true)
                {
                    // 刷新图书列表
                    await LoadBooksAsync();
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorAndShowMessage(ex, "打开编辑图书对话框时发生错误，请重试。", "BookManagementViewModel.EditBook");
            }
        }

        /// <summary>
        /// 删除图书
        /// </summary>
        private async void DeleteBooks()
        {
            var selectedItems = Books.Where(b => b.IsSelected).ToList();
            if (selectedItems.Count == 0)
            {
                _loggingService.LogWarningMessage("请选择要删除的图书", "提示");
                return;
            }

            var result = MessageBox.Show($"确定要删除选中的 {selectedItems.Count} 本图书吗？\n\n删除后将无法恢复！", "确认删除",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;

                    var bookIds = selectedItems.Select(item => item.Book.Id).ToList();

                    ApiResponse<bool> response;
                    if (bookIds.Count == 1)
                    {
                        // 单个删除
                        response = await _bookService.DeleteBookAsync(bookIds.First());
                    }
                    else
                    {
                        // 批量删除
                        response = await _bookService.DeleteBooksAsync(bookIds);
                    }

                    if (response.Success)
                    {
                        _loggingService.LogSuccessMessage($"成功删除 {bookIds.Count} 本图书！", "删除成功");

                        // 刷新图书列表
                        await LoadBooksAsync();
                    }
                    else
                    {
                        _loggingService.LogApiError(response.Message, "BookManagementViewModel.DeleteBooks", "删除图书");
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogErrorAndShowMessage(ex, "删除图书时发生异常，请重试。", "BookManagementViewModel.DeleteBooks");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        #endregion

        #region 分页方法

        /// <summary>
        /// 跳转到首页
        /// </summary>
        private async Task GoToFirstPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage = 1;
                await LoadBooksAsync();
            }
        }

        /// <summary>
        /// 跳转到上一页
        /// </summary>
        private async Task GoToPreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadBooksAsync();
            }
        }

        /// <summary>
        /// 跳转到下一页
        /// </summary>
        private async Task GoToNextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadBooksAsync();
            }
        }

        /// <summary>
        /// 跳转到末页
        /// </summary>
        private async Task GoToLastPageAsync()
        {
            if (CurrentPage < TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadBooksAsync();
            }
        }

        /// <summary>
        /// 跳转到指定页
        /// </summary>
        private async Task JumpToSpecificPageAsync()
        {
            if (JumpToPage >= 1 && JumpToPage <= TotalPages && JumpToPage != CurrentPage)
            {
                CurrentPage = JumpToPage;
                await LoadBooksAsync();
            }
            else if (JumpToPage < 1 || JumpToPage > TotalPages)
            {
                _loggingService.LogWarningMessage($"页码必须在 1 到 {TotalPages} 之间", "提示");
                JumpToPage = CurrentPage;
            }
        }

        #endregion

        #region INotifyPropertyChanged 实现

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

    /// <summary>
    /// BookVO 包装类，用于支持多选功能
    /// </summary>
    public class BookVOWrapper : INotifyPropertyChanged
    {
        private bool _isSelected;

        public BookVOWrapper(BookVO book)
        {
            Book = book;
        }

        public BookVO Book { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        // 代理 BookVO 的属性
        public int Id => Book.Id;
        public string Title => Book.Title ?? string.Empty;
        public string Author => Book.AuthorName ?? string.Empty;
        public string Isbn => Book.Isbn ?? string.Empty;
        public string Publisher => Book.PublisherName ?? string.Empty;

        public string Category => Book.CategoryNames != null && Book.CategoryNames.Count > 0
            ? string.Join(", ", Book.CategoryNames)
            : string.Empty;

        public int Stock => Book.Stock;
        public int Available => Book.Available;
        public DateTime PublishDate => Book.PublishedDate;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}