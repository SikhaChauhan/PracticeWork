using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace RepositoryLayer.Repository
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookDBContext _context;

        public UserRL(AddressBookDBContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> RegisterUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> LoginUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null; // User not found

            // Verify the password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password.Trim(), user.PasswordHash);
            if (!isPasswordValid) return null; // Invalid password

            return user; // Return user if authenticated
        }

    }
}
