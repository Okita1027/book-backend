using book_frontend.Services.Interfaces;
using book_frontend.Models;
using book_frontend.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace book_frontend.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly RelayCommand _registerCommand;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isLoading = false;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;

    public event Action? NavigateToLogin;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        _registerCommand = new RelayCommand(async () => await RegisterAsync(), () => CanRegister());
    }

    public IAuthService AuthService => _authService;

    public RelayCommand RegisterCommand => _registerCommand;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
            _registerCommand.NotifyCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
            _registerCommand.NotifyCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
            ValidatePassword();
            _registerCommand.NotifyCanExecuteChanged();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            OnPropertyChanged();
            ValidatePassword();
            _registerCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            _registerCommand.NotifyCanExecuteChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            _successMessage = value;
            OnPropertyChanged();
        }
    }

    private bool CanRegister()
    {
        return !IsLoading &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(ConfirmPassword) &&
               Password.Length >= 6 &&
               Password == ConfirmPassword &&
               IsValidEmail(Email);
    }

    private async Task RegisterAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            // 验证密码匹配
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "密码和确认密码不匹配";
                return;
            }

            // 创建用户对象
            var user = new User
            {
                Name = Name.Trim(),
                Email = Email.Trim().ToLower(),
                Password = Password.Trim(),
                Role = UserRole.User
            };

            // 调用注册服务
            var response = await _authService.RegisterAsync(user);

            if (response.Success)
            {
                SuccessMessage = UiConstant.RegisterSuccessMessage;

                // 延迟1秒后跳转到登录页面
                await Task.Delay(1000);

                // 清空表单
                ClearForm();
                NavigateToLogin?.Invoke();
            }
            else
            {
                ErrorMessage = response.Message ?? "注册失败，请重试";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"注册失败：{ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearForm()
    {
        Name = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        SuccessMessage = string.Empty;
        ErrorMessage = string.Empty;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
        catch
        {
            return false;
        }
    }

    private void ValidatePassword()
    {
        // 清除之前的错误信息（如果不是注册相关的错误）
        if (!ErrorMessage.Contains("注册失败"))
        {
            ErrorMessage = string.Empty;
        }

        // 验证密码长度
        if (!string.IsNullOrEmpty(Password) && Password.Length < 6)
        {
            ErrorMessage = "密码长度不能少于6位";
            return;
        }

        // 验证密码匹配
        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && Password != ConfirmPassword)
        {
            ErrorMessage = "两次输入的密码不一致";
            return;
        }

        // 如果验证通过，清除错误信息
        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) &&
            Password.Length >= 6 && Password == ConfirmPassword)
        {
            if (ErrorMessage is "密码长度不能少于6位" or "两次输入的密码不一致")
            {
                ErrorMessage = string.Empty;
            }
        }
    }

    /// <summary>
    /// 触发导航到登录页面的事件
    /// </summary>
    public void TriggerNavigateToLogin()
    {
        NavigateToLogin?.Invoke();
    }

    /// <summary>
    /// 重置注册表单状态
    /// </summary>
    public void ResetForm()
    {
        ClearForm();
    }
}