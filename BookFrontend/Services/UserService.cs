using book_frontend.Helpers;
using book_frontend.Models.DTOs;
using book_frontend.Models.Entities;
using book_frontend.Services.Interfaces;

namespace book_frontend.Services;

public class UserService : IUserService
{
    private readonly ApiClient _apiClient;

    public UserService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        return await _apiClient.GetAsync<List<User>>("Users");
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
    {
        return await _apiClient.GetAsync<User>($"Users/{id}");
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int id, User user)
    {
        return await _apiClient.PutAsync<User>($"Users/{id}", user);
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        return await _apiClient.DeleteAsync($"Users/{id}");
    }
}