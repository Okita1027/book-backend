using book_backend.Exceptions;
using book_backend.Models.DTO;
using book_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static book_backend.Constants.IServiceConstants;

namespace book_backend.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto">登录信息</param>
        /// <returns>认证响应</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] UserLoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = INCORRECT_FIELD_VALIDATION });
            }
            try
            {
                var ipAddress = GetIpAddress();
                var response = await _authService.LoginAsync(loginDto, ipAddress);
                
                // 设置RefreshToken到HttpOnly Cookie（可选，增强安全性）
                SetRefreshTokenCookie(response.RefreshToken!);
                
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Login failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error occurred");
                return StatusCode(500, new { message = OPERATION_FAILED });
            }
        }

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <param name="request">刷新令牌请求</param>
        /// <returns>新的认证响应</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO? request = null)
        {
            try
            {
                // 优先从请求体获取RefreshToken，其次从Cookie获取
                var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "刷新令牌不能为空" });
                }

                var ipAddress = GetIpAddress();
                var response = await _authService.RefreshTokenAsync(refreshToken, ipAddress);
                
                // 更新RefreshToken Cookie
                SetRefreshTokenCookie(response.RefreshToken!);
                
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error occurred");
                return StatusCode(500, new { message = "令牌刷新过程中发生错误" });
            }
        }

        /// <summary>
        /// 撤销刷新令牌
        /// </summary>
        /// <param name="request">撤销令牌请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDTO? request = null)
        {
            try
            {
                // 优先从请求体获取RefreshToken，其次从Cookie获取
                var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "刷新令牌不能为空" });
                }

                var ipAddress = GetIpAddress();
                await _authService.RevokeTokenAsync(refreshToken, ipAddress, request?.Reason);
                
                // 清除RefreshToken Cookie
                Response.Cookies.Delete("refreshToken");
                
                return Ok(new { message = "令牌已成功撤销" });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Token revocation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token revocation error occurred");
                return StatusCode(500, new { message = "令牌撤销过程中发生错误" });
            }
        }

        /// <summary>
        /// 撤销用户所有刷新令牌（登出所有设备）
        /// </summary>
        /// <param name="request">撤销令牌请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("revoke-all-tokens")]
        [Authorize]
        public async Task<IActionResult> RevokeAllTokens([FromBody] RevokeTokenRequestDTO? request = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "无效的用户标识" });
                }

                var ipAddress = GetIpAddress();
                await _authService.RevokeAllUserTokensAsync(userId, ipAddress, request?.Reason);
                
                // 清除RefreshToken Cookie
                Response.Cookies.Delete("refreshToken");
                
                return Ok(new { message = "所有令牌已成功撤销" });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("All tokens revocation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "All tokens revocation error occurred");
                return StatusCode(500, new { message = "令牌撤销过程中发生错误" });
            }
        }

        /// <summary>
        /// 获取当前用户信息（需要有效的JWT令牌）
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return BadRequest(new { message = "无效的用户令牌" });
                }

                return Ok(new
                {
                    id = userIdClaim,
                    name = nameClaim,
                    email = emailClaim,
                    role = roleClaim
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user error occurred");
                return StatusCode(500, new { message = "获取用户信息时发生错误" });
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        private string GetIpAddress()
        {
            // 检查是否通过代理
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
            }
            
            if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                return Request.Headers["X-Real-IP"].FirstOrDefault() ?? "unknown";
            }
            
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        /// <summary>
        /// 设置RefreshToken到HttpOnly Cookie
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // 防止XSS攻击
                Secure = true,   // 仅在HTTPS下传输
                SameSite = SameSiteMode.Strict, // 防止CSRF攻击
                Expires = DateTime.UtcNow.AddDays(7) // 与RefreshToken过期时间一致
            };
            
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}