using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO
{
    /// <summary>
    /// 刷新令牌请求DTO
    /// </summary>
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "RefreshToken不能为空")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// 撤销令牌请求DTO
    /// </summary>
    public class RevokeTokenRequestDTO
    {
        [Required(ErrorMessage = "RefreshToken不能为空")]
        public string RefreshToken { get; set; } = string.Empty;
        
        public string? Reason { get; set; } // 撤销原因（可选）
    }
}