using System;
using System.Threading.Tasks;

namespace book_frontend.Services.Interfaces
{
    /// <summary>
    /// Windows凭据管理器服务接口
    /// 用于安全存储和检索用户认证信息
    /// </summary>
    public interface ICredentialManagerService
    {
        /// <summary>
        /// 存储RefreshToken到Windows凭据管理器
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="refreshToken">RefreshToken</param>
        /// <param name="accessToken">AccessToken（可选）</param>
        /// <returns>是否存储成功</returns>
        Task<bool> SaveTokensAsync(string username, string refreshToken, string? accessToken = null);

        /// <summary>
        /// 从Windows凭据管理器检索RefreshToken
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>存储的Token信息</returns>
        Task<(string? refreshToken, string? accessToken)> GetTokensAsync(string username);

        /// <summary>
        /// 删除存储的Token信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteTokensAsync(string username);

        /// <summary>
        /// 检查是否存在存储的Token
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>是否存在</returns>
        Task<bool> HasStoredTokensAsync(string username);

        /// <summary>
        /// 获取所有存储的用户名列表（用于自动登录选择）
        /// </summary>
        /// <returns>用户名列表</returns>
        Task<List<string>> GetStoredUsernamesAsync();

        /// <summary>
        /// 清除所有存储的Token信息
        /// </summary>
        /// <returns>是否清除成功</returns>
        Task<bool> ClearAllTokensAsync();
    }
}