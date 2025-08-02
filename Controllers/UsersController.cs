using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        // GET: api/Users
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // 普通用户看不到自己加密后的密码
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            if (currentUserRole == nameof(UserRole.Member))
            {
                user.PasswordHash = null;
            }
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, [FromBody] EditUserDTO editUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usersService.UpdateUserAsync(id, editUserDTO);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Users/register
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="editUserDTO">用户名称、邮箱、密码</param>
        /// <returns>注册结果</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] EditUserDTO editUserDTO)
        {
            try
            {
                var user = await _usersService.RegisterUserAsync(editUserDTO);
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Users/login
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns>Token、用户名称、角色、过期时间</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginUser([FromBody] UserLoginDTO userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string email = userLoginDto.Email;
            string password = userLoginDto.Password;
            try
            {
                AuthResponseDTO authResponseDto = await _usersService.LoginUserAsync(email, password);
                return Ok(authResponseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _usersService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 批量删除用户
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteUsers([FromBody] List<int> ids)
        {
            try
            {
                await _usersService.DeleteUsersAsync(ids);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}