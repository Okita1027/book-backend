using book_frontend.Models;

namespace book_frontend.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>登录结果</returns>
    Task<LoginResponse> LoginAsync(string username, string password);
    
    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="password">密码</param>
    /// <returns>注册结果</returns>
    Task<ApiResponse<User>> RegisterAsync(User user, string password);
    
    /// <summary>
    /// 登出
    /// </summary>
    Task LogoutAsync();
    
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
}
