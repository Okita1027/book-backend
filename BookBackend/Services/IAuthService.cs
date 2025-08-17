using book_backend.Models.DTO;
using book_backend.Models.Entity;

namespace book_backend.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto">登录信息</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <returns>认证响应</returns>
        Task<AuthResponseDTO> LoginAsync(UserLoginDTO loginDto, string ipAddress);

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <returns>新的认证响应</returns>
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress);

        /// <summary>
        /// 撤销刷新令牌
        /// </summary>
        /// <param name="refreshToken">要撤销的刷新令牌</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="reason">撤销原因</param>
        /// <returns></returns>
        Task RevokeTokenAsync(string refreshToken, string ipAddress, string? reason = null);

        /// <summary>
        /// 撤销用户的所有刷新令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="reason">撤销原因</param>
        /// <returns></returns>
        Task RevokeAllUserTokensAsync(int userId, string ipAddress, string? reason = null);

        /// <summary>
        /// 验证刷新令牌是否有效
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>令牌对应的用户，如果无效则返回null</returns>
        Task<User?> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// 生成JWT访问令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>JWT令牌</returns>
        string GenerateJwtToken(User user);

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <returns>刷新令牌实体</returns>
        Task<RefreshToken> GenerateRefreshTokenAsync(User user, string ipAddress);
    }
}