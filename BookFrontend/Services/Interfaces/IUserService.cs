using book_frontend.Models;

namespace book_frontend.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    Task<ApiResponse<List<User>>> GetAllUsersAsync();
    
    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    Task<ApiResponse<User>> GetUserByIdAsync(int id);
    
    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="user">用户信息</param>
    /// <returns>更新结果</returns>
    Task<ApiResponse<User>> UpdateUserAsync(int id, User user);
    
    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
}
