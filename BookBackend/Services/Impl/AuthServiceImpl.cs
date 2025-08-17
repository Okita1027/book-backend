using book_backend.Data;
using book_backend.Exceptions;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using Masuit.Tools.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace book_backend.Services.Impl
{
    /// <summary>
    /// 认证服务实现类
    /// </summary>
    public class AuthServiceImpl : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthServiceImpl> _logger;

        public AuthServiceImpl(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AuthServiceImpl> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> LoginAsync(UserLoginDTO loginDto, string ipAddress)
        {
            // 验证用户凭据
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // 使用MD5验证密码（兼容现有数据库中的MD5密码）
            if (user == null || user.PasswordHash != loginDto.Password.MDString())
            {
                _logger.LogWarning("Login failed for email: {Email} from IP: {IpAddress}", loginDto.Email, ipAddress);
                throw new BusinessException("邮箱或密码错误");
            }

            // 生成JWT访问令牌
            var jwtToken = GenerateJwtToken(user);

            // 生成刷新令牌
            var refreshToken = await GenerateRefreshTokenAsync(user, ipAddress);

            // 保存刷新令牌到数据库
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} logged in successfully from IP: {IpAddress}", user.Id, ipAddress);

            return new AuthResponseDTO
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Id = user.Id,
                Name = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1), // AccessToken 1小时过期
                RefreshExpiresAt = refreshToken.ExpiresAt
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var user = await ValidateRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                _logger.LogWarning("Invalid refresh token used from IP: {IpAddress}", ipAddress);
                throw new BusinessException("无效的刷新令牌");
            }

            // 撤销旧的刷新令牌
            var oldRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            
            if (oldRefreshToken != null)
            {
                oldRefreshToken.IsRevoked = true;
                oldRefreshToken.RevokedAt = DateTime.UtcNow;
                oldRefreshToken.RevokedByIp = ipAddress;
                oldRefreshToken.ReasonRevoked = "Token refreshed";
            }

            // 生成新的JWT访问令牌
            var jwtToken = GenerateJwtToken(user);

            // 生成新的刷新令牌
            var newRefreshToken = await GenerateRefreshTokenAsync(user, ipAddress);

            // 保存新的刷新令牌
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Token refreshed for user {UserId} from IP: {IpAddress}", user.Id, ipAddress);

            return new AuthResponseDTO
            {
                Token = jwtToken,
                RefreshToken = newRefreshToken.Token,
                Id = user.Id,
                Name = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1), // AccessToken 1小时过期
                RefreshExpiresAt = newRefreshToken.ExpiresAt
            };
        }

        public async Task RevokeTokenAsync(string refreshToken, string ipAddress, string? reason = null)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null)
            {
                throw new BusinessException("令牌不存在");
            }

            if (token.IsRevoked)
            {
                throw new BusinessException("令牌已被撤销");
            }

            // 撤销令牌
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason ?? "Token revoked by user";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked for user {UserId} from IP: {IpAddress}, Reason: {Reason}", 
                token.UserId, ipAddress, reason);
        }

        public async Task RevokeAllUserTokensAsync(int userId, string ipAddress, string? reason = null)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                token.ReasonRevoked = reason ?? "All tokens revoked";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("All refresh tokens revoked for user {UserId} from IP: {IpAddress}, Count: {Count}", 
                userId, ipAddress, tokens.Count);
        }

        public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            return token != null && !token.IsRevoked && !token.IsExpired ? token.User : null;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // AccessToken 1小时过期
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(User user, string ipAddress)
        {
            // 生成安全的随机令牌
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            // 确保令牌唯一性
            while (await _context.RefreshTokens.AnyAsync(rt => rt.Token == token))
            {
                rng.GetBytes(randomBytes);
                token = Convert.ToBase64String(randomBytes);
            }

            return new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // RefreshToken 7天过期
                CreatedByIp = ipAddress
            };
        }
    }
}