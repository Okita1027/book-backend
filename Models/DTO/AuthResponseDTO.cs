using DEMO_CRUD.Models.Entity;

namespace DEMO_CRUD.Models.DTO;

public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = nameof(UserRole.Member);

    // 可以根据需要添加其他用户信息，如用户ID、角色列表等
    public DateTime ExpiresAt { get; set; } // 令牌过期时间
}