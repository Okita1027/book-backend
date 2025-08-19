using book_frontend.Models.Entities;
using book_frontend.Services.Interfaces;
using book_frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace book_frontend.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ILogger<AdminViewModel> _logger;
    [ObservableProperty]
    private string _currentPageTitle = "欢迎";
    
    [ObservableProperty]
    private string _currentUserName = string.Empty;

    public AdminViewModel(IAuthService authService, ILogger<AdminViewModel> logger)
    {
        _authService = authService;
        _logger = logger;
        
        // 获取当前用户信息
        LoadCurrentUser();
    }

    /// <summary>
    /// 检查是否有管理员权限
    /// </summary>
    /// <returns>是否有管理员权限</returns>
    public bool HasAdminPermission()
    {
        try
        {
            // 首先检查是否已登录
            if (!_authService.IsLoggedIn())
            {
                _logger.LogWarning("User is not logged in when checking admin permission");
                return false;
            }

            // 获取当前用户信息
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null when checking admin permission");
                return false;
            }

            // 检查用户是否为管理员
            var isAdmin = IsUserAdmin(currentUser);
            
            if (!isAdmin)
            {
                _logger.LogWarning("User {Email} does not have admin permission", currentUser.Email);
            }
            
            return isAdmin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking admin permission");
            return false;
        }
    }

    /// <summary>
    /// 判断用户是否为管理员
    /// </summary>
    /// <param name="user">用户对象</param>
    /// <returns>是否为管理员</returns>
    private static bool IsUserAdmin(User user)
    {
        // 使用User模型的Role属性判断管理员权限
        return user.Role == UserRole.Admin;
    }

    /// <summary>
    /// 加载当前用户信息
    /// </summary>
    private void LoadCurrentUser()
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser != null)
            {
                CurrentUserName = currentUser.Name;
            }
            else
            {
                CurrentUserName = "未登录";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current user information");
            CurrentUserName = "加载失败";
        }
    }

    /// <summary>
    /// 刷新当前用户信息
    /// </summary>
    public void RefreshCurrentUser()
    {
        LoadCurrentUser();
    }
}