using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEMO_CRUD.Data;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Models.DTO;
using Masuit.Tools.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, EditUserDTO editUserDTO)
        {
            // 1.判断请求数据是否有效【符合注解的要求】
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2.查找现有实体
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("此用户不存在！");
            }

            // 3.更新现有实体的属性
            existingUser.Name = editUserDTO.Name;
            existingUser.Email = editUserDTO.Email;
            existingUser.PasswordHash = editUserDTO.PasswordHash.MDString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Users
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(EditUserDTO editUserDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Email == editUserDTO.Email)) // 使用 Email 作为唯一标识
            {
                return BadRequest("该邮箱已被注册!");
            }

            var user = new User
            {
                Name = editUserDTO.Name,
                Email = editUserDTO.Email,
                PasswordHash = editUserDTO.PasswordHash.MDString(),
                RegistrationDate = DateTime.Now,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <returns>Token、用户名称、角色、过期时间</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("用户不存在");
            }

            // 使用 MD5 比较密码
            if (user.PasswordHash != password.MDString())
            {
                return Unauthorized("密码错误");
            }

            // 登录成功,生成JWT TOKEN
            (string Token, DateTime ExpiresAt) jwtToken = GenerateJwtToken(user);
            return Ok(new AuthResponseDTO()
            {
                Token = jwtToken.Token,
                Name = user.Name,
                Role = user.Role.ToString(),
                ExpiresAt = jwtToken.ExpiresAt
            });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        /// <summary>
        /// 生成JWT Token的辅助方法
        /// </summary>
        /// <param name="user">用户的信息</param>
        /// <returns>TOKEN和过期时间</returns>
        private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;

            // 加密算法需要字节数组形式的密钥
            var key = Encoding.ASCII.GetBytes(secretKey);

            // JWT 声明 (Claims)：这些信息将包含在令牌中
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                // 可以根据需要添加其他自定义声明，如用户角色(一般用户/管理员)
                new Claim(ClaimTypes.Role, user.Role.ToString()),
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
            var expires = DateTime.Now.AddHours(1); // 设置令牌有效期为 1 小时

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
}