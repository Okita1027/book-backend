using System.Windows.Input;
using book_frontend.Commands;
using book_frontend.Services.Interfaces;

namespace book_frontend.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _isLoading = false;
    private string _errorMessage = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // 命令——类似React的事件处理函数
    public ICommand LoginCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        // 初始化命令
        LoginCommand = null;
    }
    // 执行登录逻辑
    private async void ExecuteLogin(object? parameter)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;


    }

}