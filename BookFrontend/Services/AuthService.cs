using book_frontend.Helpers;
using book_frontend.Models;
using book_frontend.Services.Interfaces;
using System.Text.Json;

namespace book_frontend.Services;

public class AuthService : IAuthService
{
    private readonly ApiClient _apiClient;
    private User? _currentUser;
    private string? _currentToken;

    public AuthService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = username,
                Password = password
            };

            // 后端返回的是AuthResponseDTO格式，需要转换为LoginResponse
            var response = await _apiClient.PostAsync<AuthResponseDTO>("Users/login", loginRequest);
            
            if (response is { Success: true, Data: not null })
            {
                var authResponse = response.Data;
                // 创建用户对象
                var user = new User
                {
                    Id = authResponse.Id,
                    Name = authResponse.Name,
                    Email = loginRequest.Email,
                    Role = authResponse.Role == "Admin" ? UserRole.Admin : UserRole.User
                };
                
                _currentUser = user;
                _currentToken = authResponse.Token;
                _apiClient.SetAuthToken(_currentToken);
                
                // 可以在这里保存到本地存储
                SaveTokenToLocalStorage(_currentToken);
                
                return new LoginResponse
                {
                    IsSuccess = true,
                    Token = authResponse.Token,
                    User = user,
                    Id = authResponse.Id,
                    Name = authResponse.Name,
                    Role = authResponse.Role,
                    ExpiresAt = authResponse.ExpiresAt
                };
            }
            else
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    ErrorMessage = response.Message ?? "登录失败"
                };
            }
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = $"登录出错: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<User>> RegisterAsync(User user, string password)
    {
        try
        {
            var registerRequest = new
            {
                user.Name,
                user.Email,
                Password = password,
                user.Role
            };

            return await _apiClient.PostAsync<User>("Users/register", registerRequest);
        }
        catch (Exception ex)
        {
            return new ApiResponse<User>
            {
                Success = false,
                Message = $"注册出错: {ex.Message}"
            };
        }
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        _currentToken = null;
        _apiClient.SetAuthToken(null);
        
        // 清除本地存储
        ClearTokenFromLocalStorage();
        
        return Task.CompletedTask;
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(_currentToken) && _currentUser != null;
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }

    public string? GetToken()
    {
        return _currentToken;
    }

    /// <summary>
    /// 保存Token到本地存储（简单实现，实际项目中可能需要加密）
    /// </summary>
    /// <param name="token">JWT Token</param>
    private static void SaveTokenToLocalStorage(string? token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            // 这里可以使用Windows注册表或文件存储
            // 为了简单起见，暂时不实现持久化
        }
    }

    /// <summary>
    /// 从本地存储清除Token
    /// </summary>
    private static void ClearTokenFromLocalStorage()
    {
        // 清除本地存储的Token
    }
}