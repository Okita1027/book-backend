using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DEMO_CRUD.Data;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using Mapster;
using Masuit.Tools.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static DEMO_CRUD.Constants.IServiceConstants;


namespace DEMO_CRUD.Services.Impl;

public class UsersServiceImpl : IUsersService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UsersServiceImpl(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> RegisterUserAsync(EditUserDTO editUserDTO)
    {
        if (await _context.Users.AnyAsync(u => u.Email == editUserDTO.Email))
        {
            throw new ArgumentException(EMAIL_ALREADY_REGISTERED);
        }

        // Adapt<T>()用于运行时对象映射
        User user = editUserDTO.Adapt<User>();
        user.RegistrationDate = DateTime.Now;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<AuthResponseDTO> LoginUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        // MD5校验密码
        if (user == null || user.PasswordHash != password.MDString())
        {
            throw new ArgumentException(USERNAME_PASSWORD_INCORRECT);
        }
        // 登录成功,生成JWT TOKEN
        (string Token, DateTime ExpiresAt) jwtToken = GenerateJwtToken(user);
        return new AuthResponseDTO
        {
            Token = jwtToken.Token,
            Id = user.Id,
            Name = email,
            Role = user.Role.ToString(),
            ExpiresAt = jwtToken.ExpiresAt
        };
    }

    public async Task UpdateUserAsync(int id, EditUserDTO editUserDTO)
    {
        User existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            throw new ArgumentException(USER_NOT_FOUND);
        }

        editUserDTO.Adapt(existingUser);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new ArgumentException(USER_NOT_FOUND);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUsersAsync(List<int> ids)
    {
        List<User> users = await _context.Users.Where(user => ids.Contains(user.Id)).ToListAsync();
        if (users.Count == 0)
        {
            throw new ArgumentException(RECORD_NOT_FOUND);
        }
        _context.Users.RemoveRange(users);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 生成JWT Token的辅助方法
    /// </summary>
    /// <param name="user">用户的信息</param>
    /// <returns>【元组】TOKEN和过期时间</returns>
    private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;

        // 加密算法需要字节数组形式的密钥
        byte[] key = Encoding.ASCII.GetBytes(secretKey);

        // JWT 声明 (Claims)：这些信息将包含在令牌中
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            // 可以根据需要添加其他自定义声明，如用户角色(一般用户/管理员)
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        /*
         // 集合表达式写法（C#12）
         Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        ];*/

        // 创建、读取和验证 JWT 令牌的核心类
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        DateTime expires = DateTime.Now.AddHours(1);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            // 令牌主体
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            // 签名凭证（防篡改）
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            // 令牌颁发者
            Issuer = issuer,
            // 令牌的受众
            Audience = audience
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expires);
    }
}