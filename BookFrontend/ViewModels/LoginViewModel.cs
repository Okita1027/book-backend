using System.Windows.Input;
using book_frontend.Commands;
using book_frontend.Services.Interfaces;

namespace book_frontend.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly RelayCommand _loginCommand;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _isLoading = false;
    private string _errorMessage = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            // 当用户名变化后，尝试更新绑定属性，并刷新命令可用状态
            if (SetProperty(ref _username, value))
            {
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            // 当密码变化后，尝试更新绑定属性，并刷新命令可用状态
            if (SetProperty(ref _password, value))
            {
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            // 登录过程中禁用按钮，结束后恢复
            if (SetProperty(ref _isLoading, value))
            {
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // 将公开的命令暴露为 ICommand，供 XAML 绑定
    public RelayCommand LoginCommand => _loginCommand;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        // 初始化命令：
        // - execute：调用异步登录方法
        // - canExecute：根据输入与加载状态决定按钮是否可用
        _loginCommand = new RelayCommand(
            execute: async _ => await ExecuteLoginAsync(),
            canExecute: _ => CanLogin()
        );
    }

    // 判断登录按钮是否可用：用户名/密码非空，且不在加载中
    private bool CanLogin()
    {
        return !_isLoading
               && !string.IsNullOrWhiteSpace(_username)
               && !string.IsNullOrWhiteSpace(_password);
    }

    // 执行登录流程
    private async Task ExecuteLoginAsync()
    {
        // 显示加载状态，并清空上一次错误信息
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // 调用认证服务进行登录
            var result = await _authService.LoginAsync(Username, Password);

            if (result.IsSuccess)
            {
                // 登录成功：
                // - 这里暂不做页面跳转，下一步我们会实现导航到图书列表页
                // - 你可以在这里设置欢迎消息或将登录状态暴露给UI
                ErrorMessage = string.Empty;
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
            // 一定要恢复加载状态
            IsLoading = false;
        }
    }


}