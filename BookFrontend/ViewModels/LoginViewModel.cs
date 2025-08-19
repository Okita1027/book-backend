using book_frontend.Services.Interfaces;
using book_frontend.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace book_frontend.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly RelayCommand _loginCommand;
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;    
    
    [ObservableProperty]
    private string _successMessage = string.Empty;
    
    [ObservableProperty]
    private bool _rememberMe = false;

    /// <summary>
    /// 事件声明，用于实现ViewModel与View之间的通信
    /// </summary>
    public event Action? NavigateToRegister;
    public event Action? LoginSuccessful;

    partial void OnEmailChanged(string value)
    {
        _loginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string value)
    {
        _loginCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsLoadingChanged(bool value)
    {
        _loginCommand.NotifyCanExecuteChanged();
    }

    // 将公开的命令暴露为 ICommand，供 XAML 绑定
    // => 是表达式体定义操作符，表示这个属性的getter直接返回 _loginCommand 字段的值
    public RelayCommand LoginCommand => _loginCommand;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        // 初始化命令：
        // execute：调用异步登录方法
        // canExecute：根据输入与加载状态决定按钮是否可用
        _loginCommand = new RelayCommand(
            // 异步执行登录方法
            execute: async void () => await ExecuteLoginAsync(),
            canExecute: CanLogin
        );
    }

    // 判断登录按钮是否可用：用户名/密码非空，且不在加载中
    private bool CanLogin()
    {
        return !IsLoading
               && !string.IsNullOrWhiteSpace(Email)
               && !string.IsNullOrWhiteSpace(Password);
    }

    // 执行登录流程
    private async Task ExecuteLoginAsync()
    {
        // 显示加载状态，并清空上一次错误信息
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // 调用认证服务进行登录，传递RememberMe参数
            var result = await _authService.LoginAsync(Email, Password, RememberMe);

            if (result.IsSuccess)
            {
                // 登录成功：清空错误信息并触发登录成功事件
                SuccessMessage = "登录成功！";
                ErrorMessage = string.Empty;
                LoginSuccessful?.Invoke();
            }
            else
            {
                // 登录失败，显示后端返回的错误信息
                ErrorMessage = result.ErrorMessage ?? "登录失败，请检查用户名或密码。";
            }
        }
        catch (Exception ex)
        {
            // 捕获意外错误，显示友好提示
            ErrorMessage = $"登录异常：{ex.Message}";
        }
        finally
        {
            // 恢复加载状态
            IsLoading = false;
        }
    }

    /// <summary>
    /// 触发导航到注册页面的事件
    /// </summary>
    public void TriggerNavigateToRegister()
    {
        NavigateToRegister?.Invoke();
    }
}