using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace book_frontend.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<MainViewModel> _logger;
    
    public BookListViewModel Home { get; }

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
                // 可以在这里触发UI更新或导航到主页面
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
}