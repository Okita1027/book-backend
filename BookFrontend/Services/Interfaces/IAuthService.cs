using book_frontend.Models;

namespace book_frontend.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    /// <param name="rememberMe">是否记住登录状态</param>
    /// <returns>登录结果</returns>
    Task<LoginResponse> LoginAsync(string email, string password, bool rememberMe = false);
    
    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns>注册结果</returns>
    Task<ApiResponse<User>> RegisterAsync(User user);
    
    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="revokeRefreshToken">是否撤销RefreshToken</param>
    Task LogoutAsync(bool revokeRefreshToken = true);
    
    /// <summary>
    /// 检查是否已登录
    /// </summary>
    /// <returns>是否已登录</returns>
    bool IsLoggedIn();
    
    /// <summary>
    /// 获取当前用户
    /// </summary>
    /// <returns>当前用户信息</returns>
    User? GetCurrentUser();
    
    /// <summary>
    /// 获取JWT Token
    /// </summary>
    /// <returns>JWT Token</returns>
    string? GetToken();
    
    /// <summary>
    /// 尝试自动登录（使用存储的RefreshToken）
    /// </summary>
    /// <returns>自动登录结果</returns>
    Task<LoginResponse> TryAutoLoginAsync();
    
    /// <summary>
    /// 获取所有存储的用户名（用于登录页面显示）
    /// </summary>
    /// <returns>用户名列表</returns>
    Task<List<string>> GetStoredUsernamesAsync();
    
    /// <summary>
    /// 清除指定用户的存储凭据
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>是否清除成功</returns>
    Task<bool> ClearStoredCredentialsAsync(string username);
    
    /// <summary>
    /// 清除所有存储的凭据
    /// </summary>
    /// <returns>是否清除成功</returns>
    Task<bool> ClearAllStoredCredentialsAsync();
}
