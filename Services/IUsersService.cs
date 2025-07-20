using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;

namespace DEMO_CRUD.Services;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> RegisterUserAsync(EditUserDTO editUserDTO);
    Task<AuthResponseDTO> LoginUserAsync(string email, string password);
    Task UpdateUserAsync(int id, EditUserDTO editUserDTO);
    Task DeleteUserAsync(int id);
    Task DeleteUsersAsync(List<int> ids);
}