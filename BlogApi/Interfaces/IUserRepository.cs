using BlogApi.Data.Entities;
using BlogApi.V1.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace BlogApi.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> CreateUserAsync(User user);
        public Task<User?> GetUserByIdAsync(int id);
        public Task<User?> UpdateUserAsync(UpdateUserRequest request, int id);
        public Task<User?> DeleteUserAsync(int id);
        public Task SaveChangesAsync();
    }
}
