using book_frontend.Services.Interfaces;
using CredentialManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace book_frontend.Services
{
    /// <summary>
    /// Windows凭据管理器服务实现
    /// 使用Windows凭据管理器安全存储RefreshToken
    /// </summary>
    public class CredentialManagerService : ICredentialManagerService
    {
        private readonly ILogger<CredentialManagerService> _logger;
        private const string TARGET_PREFIX = "BookManagement_";
        private const string REFRESH_TOKEN_SUFFIX = "_RefreshToken";
        private const string ACCESS_TOKEN_SUFFIX = "_AccessToken";

        /// <summary>
        /// Token存储数据结构
        /// </summary>
        private class TokenData
        {
            public string? RefreshToken { get; set; }
            public string? AccessToken { get; set; }
            public DateTime StoredAt { get; set; } = DateTime.UtcNow;
        }

        public CredentialManagerService(ILogger<CredentialManagerService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SaveTokensAsync(string username, string refreshToken, string? accessToken = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("Username or RefreshToken is empty");
                    return false;
                }

                var tokenData = new TokenData
                {
                    RefreshToken = refreshToken,
                    AccessToken = accessToken
                };

                var jsonData = JsonSerializer.Serialize(tokenData);
                var target = GetCredentialTarget(username);

                using var credential = new Credential
                {
                    Target = target,
                    Username = username,
                    Password = jsonData,
                    Type = CredentialType.Generic,
                    PersistanceType = PersistanceType.LocalComputer,
                    Description = "BookManagement RefreshToken Storage"
                };

                var result = credential.Save();
                
                if (result)
                {
                    _logger.LogInformation("Tokens saved successfully for user: {Username}", username);
                }
                else
                {
                    _logger.LogError("Failed to save tokens for user: {Username}", username);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tokens for user: {Username}", username);
                return false;
            }
        }

        public async Task<(string? refreshToken, string? accessToken)> GetTokensAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    _logger.LogWarning("Username is empty");
                    return (null, null);
                }

                var target = GetCredentialTarget(username);

                using var credential = new Credential { Target = target };
                
                if (!credential.Load())
                {
                    _logger.LogDebug("No stored tokens found for user: {Username}", username);
                    return (null, null);
                }

                if (string.IsNullOrWhiteSpace(credential.Password))
                {
                    _logger.LogWarning("Stored credential data is empty for user: {Username}", username);
                    return (null, null);
                }

                var tokenData = JsonSerializer.Deserialize<TokenData>(credential.Password);
                
                if (tokenData == null)
                {
                    _logger.LogWarning("Failed to deserialize token data for user: {Username}", username);
                    return (null, null);
                }

                _logger.LogDebug("Tokens retrieved successfully for user: {Username}", username);
                return await Task.FromResult((tokenData.RefreshToken, tokenData.AccessToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tokens for user: {Username}", username);
                return (null, null);
            }
        }

        public async Task<bool> DeleteTokensAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    _logger.LogWarning("Username is empty");
                    return false;
                }

                var target = GetCredentialTarget(username);

                using var credential = new Credential { Target = target };
                
                var result = credential.Delete();
                
                if (result)
                {
                    _logger.LogInformation("Tokens deleted successfully for user: {Username}", username);
                }
                else
                {
                    _logger.LogWarning("No tokens found to delete for user: {Username}", username);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tokens for user: {Username}", username);
                return false;
            }
        }

        public async Task<bool> HasStoredTokensAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return false;
                }

                var target = GetCredentialTarget(username);

                using var credential = new Credential { Target = target };
                
                var exists = credential.Exists();
                
                _logger.LogDebug("Token existence check for user {Username}: {Exists}", username, exists);
                
                return await Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token existence for user: {Username}", username);
                return false;
            }
        }

        public async Task<List<string>> GetStoredUsernamesAsync()
        {
            try
            {
                // CredentialManagement包中没有LoadAll方法，我们需要使用其他方式
                // 这里返回空列表，实际使用中可能需要其他实现方式
                var usernames = new List<string>();
                
                _logger.LogDebug("Found {Count} stored usernames", usernames.Count);
                
                return await Task.FromResult(usernames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stored usernames");
                return new List<string>();
            }
        }

        public async Task<bool> ClearAllTokensAsync()
        {
            try
            {
                // CredentialManagement包中没有LoadAll方法，我们需要使用其他方式
                // 这里暂时返回true，实际使用中可能需要其他实现方式
                var credentials = new List<Credential>();

                var deletedCount = 0;
                
                foreach (var credential in credentials)
                {
                    try
                    {
                        if (credential.Delete())
                        {
                            deletedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete credential: {Target}", credential.Target);
                    }
                    finally
                    {
                        credential.Dispose();
                    }
                }

                _logger.LogInformation("Cleared {DeletedCount} out of {TotalCount} stored tokens", deletedCount, credentials.Count);
                
                return await Task.FromResult(deletedCount == credentials.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all tokens");
                return false;
            }
        }

        /// <summary>
        /// 生成凭据目标名称
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>凭据目标名称</returns>
        private static string GetCredentialTarget(string username)
        {
            return $"{TARGET_PREFIX}{username}";
        }
    }
}