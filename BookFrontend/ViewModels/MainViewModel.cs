using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<MainViewModel> _logger;
    
    public BookListViewModel Home { get; }
    
    /// <summary>
    /// 获取当前用户是否已登录
    /// </summary>
    public bool IsLoggedIn => _authService.IsLoggedIn();

    public MainViewModel(IBookService bookService, IAuthService authService, ILogger<MainViewModel> logger)
    {
        _authService = authService;
        _logger = logger;
        Home = new BookListViewModel(bookService);
        
        // 异步初始化自动登录
        _ = InitializeAsync();
    }
    
    private async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Attempting auto login...");
            var autoLoginResult = await _authService.TryAutoLoginAsync();
            
            if (autoLoginResult.IsSuccess)
            {
                _logger.LogInformation("Auto login successful for user: {UserName}", autoLoginResult.Name);
                // 触发登录状态变更通知
                OnPropertyChanged(nameof(IsLoggedIn));
            }
            else
            {
                _logger.LogDebug("Auto login failed: {ErrorMessage}", autoLoginResult.ErrorMessage);
                // 用户需要手动登录
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto login initialization");
        }
    }
    
    /// <summary>
    /// 通知登录状态已更改
    /// </summary>
    public void NotifyLoginStateChanged()
    {
        OnPropertyChanged(nameof(IsLoggedIn));
    }
}