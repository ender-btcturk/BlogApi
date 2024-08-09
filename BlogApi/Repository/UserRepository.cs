using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.V1.Requests.User;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BlogDbContext _context;
        public UserRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> UpdateUserAsync(UpdateUserRequest request, int id)
        {
            User? existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            _context.Users
                    .Entry(existingUser)
                    .CurrentValues
                    .SetValues(request);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
