using book_backend.Models.DTO;
using book_backend.Models.Entity;

namespace book_backend.Services;

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