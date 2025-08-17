using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.Entities;
using book_frontend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace book_frontend.Services;

public class AuthService : IAuthService
{
    private readonly ApiClient _apiClient;
    private readonly ICredentialManagerService _credentialManager;
    private readonly ILogger<AuthService> _logger;
    private User? _currentUser;
    private string? _currentToken;
    private string? _currentRefreshToken;

    public AuthService(ApiClient apiClient, ICredentialManagerService credentialManager, ILogger<AuthService> logger)
    {
        _apiClient = apiClient;
        _credentialManager = credentialManager;
        _logger = logger;
        
        // 订阅ApiClient的Token刷新事件
        _apiClient.TokenRefreshed += OnTokenRefreshed;
        _apiClient.AuthenticationFailed += OnAuthenticationFailed;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            // 后端返回的是AuthResponseDTO格式，需要转换为LoginResponse
            var response = await _apiClient.PostAsync<AuthResponse>("Auth/login", loginRequest);
            
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
                
                // 更新当前状态
                _currentUser = user;
                _currentToken = authResponse.Token;
                _currentRefreshToken = authResponse.RefreshToken;
                
                // 设置ApiClient的Token
                _apiClient.SetTokens(authResponse.Token, authResponse.RefreshToken, authResponse.ExpiresAt);
                
                // 如果选择记住登录状态，保存到Windows凭据管理器
                if (rememberMe && !string.IsNullOrEmpty(authResponse.RefreshToken))
                {
                    await _credentialManager.SaveTokensAsync(user.Name, authResponse.RefreshToken, authResponse.Token);
                    _logger.LogInformation("Tokens saved to credential manager for user: {Username}", user.Name);
                }
                
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

            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = response.Message ?? "登录失败"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user: {Email}", email);
            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = $"登录出错: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<User>> RegisterAsync(User user)
    {
        try
        {
            var registerRequest = new
            {
                user.Name,
                user.Email,
                user.Password
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

    public async Task LogoutAsync(bool revokeRefreshToken = true)
    {
        try
        {
            // 如果需要撤销RefreshToken且当前有RefreshToken
            if (revokeRefreshToken && !string.IsNullOrEmpty(_currentRefreshToken))
            {
                try
                {
                    var revokeRequest = new { RefreshToken = _currentRefreshToken };
                    await _apiClient.PostAsync<object>("Auth/revoke-token", revokeRequest);
                    _logger.LogInformation("RefreshToken revoked successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to revoke RefreshToken during logout");
                    // 继续执行登出流程，即使撤销失败
                }
            }
            
            // 清除存储的凭据（如果当前用户存在）
            if (_currentUser != null)
            {
                await _credentialManager.DeleteTokensAsync(_currentUser.Name);
            }
            
            // 清除内存中的状态
            _currentUser = null;
            _currentToken = null;
            _currentRefreshToken = null;
            
            // 清除ApiClient的Token
            _apiClient.ClearTokens();
            
            _logger.LogInformation("User logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            // 即使出错也要清除本地状态
            _currentUser = null;
            _currentToken = null;
            _currentRefreshToken = null;
            _apiClient.ClearTokens();
        }
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
    
    public async Task<LoginResponse> TryAutoLoginAsync()
    {
        try
        {
            var storedUsernames = await _credentialManager.GetStoredUsernamesAsync();
            
            // 尝试使用第一个存储的用户进行自动登录
            foreach (var username in storedUsernames)
            {
                var (refreshToken, accessToken) = await _credentialManager.GetTokensAsync(username);
                
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        // 使用RefreshToken获取新的访问Token
                        var refreshRequest = new { RefreshToken = refreshToken };
                        var response = await _apiClient.PostAsync<AuthResponse>("Auth/refresh-token", refreshRequest);
                        
                        if (response is { Success: true, Data: not null })
                        {
                            var authResponse = response.Data;
                            
                            // 创建用户对象（这里需要从Token中解析或从其他地方获取用户信息）
                            var user = new User
                            {
                                Id = authResponse.Id,
                                Name = authResponse.Name,
                                Email = username, // 假设username就是email
                                Role = authResponse.Role == "Admin" ? UserRole.Admin : UserRole.User
                            };
                            
                            // 更新当前状态
                            _currentUser = user;
                            _currentToken = authResponse.Token;
                            _currentRefreshToken = authResponse.RefreshToken;
                            
                            // 设置ApiClient的Token
                            _apiClient.SetTokens(authResponse.Token, authResponse.RefreshToken, authResponse.ExpiresAt);
                            
                            // 更新存储的Token
                            await _credentialManager.SaveTokensAsync(username, authResponse.RefreshToken, authResponse.Token);
                            
                            _logger.LogInformation("Auto login successful for user: {Username}", username);
                            
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

                        // RefreshToken可能已过期，删除存储的凭据
                        await _credentialManager.DeleteTokensAsync(username);
                        _logger.LogWarning("RefreshToken expired for user: {Username}, credentials removed", username);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Auto login failed for user: {Username}", username);
                        // 继续尝试下一个用户
                    }
                }
            }
            
            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = "没有可用的自动登录凭据"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auto login process failed");
            return new LoginResponse
            {
                IsSuccess = false,
                ErrorMessage = $"自动登录出错: {ex.Message}"
            };
        }
    }
    
    public async Task<List<string>> GetStoredUsernamesAsync()
    {
        try
        {
            return await _credentialManager.GetStoredUsernamesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get stored usernames");
            return new List<string>();
        }
    }
    
    public async Task<bool> ClearStoredCredentialsAsync(string username)
    {
        try
        {
            return await _credentialManager.DeleteTokensAsync(username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear credentials for user: {Username}", username);
            return false;
        }
    }
    
    public async Task<bool> ClearAllStoredCredentialsAsync()
    {
        try
        {
            return await _credentialManager.ClearAllTokensAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear all stored credentials");
            return false;
        }
    }
    
    /// <summary>
    /// 处理Token刷新事件
    /// </summary>
    /// <param name="newAccessToken">新的访问Token</param>
    /// <param name="newRefreshToken">新的刷新Token</param>
    private async void OnTokenRefreshed(string newAccessToken, string newRefreshToken)
    {
        try
        {
            _currentToken = newAccessToken;
            _currentRefreshToken = newRefreshToken;
            
            // 更新存储的Token（如果当前用户存在）
            if (_currentUser != null)
            {
                await _credentialManager.SaveTokensAsync(_currentUser.Name, newRefreshToken, newAccessToken);
                _logger.LogDebug("Tokens updated in credential manager for user: {Username}", _currentUser.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle token refresh event");
        }
    }
    
    /// <summary>
    /// 处理认证失败事件
    /// </summary>
    private async void OnAuthenticationFailed()
    {
        try
        {
            _logger.LogWarning("Authentication failed, clearing user session");
            
            // 清除存储的凭据（如果当前用户存在）
            if (_currentUser != null)
            {
                await _credentialManager.DeleteTokensAsync(_currentUser.Name);
            }
            
            // 清除内存中的状态
            _currentUser = null;
            _currentToken = null;
            _currentRefreshToken = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle authentication failure event");
        }
    }
}