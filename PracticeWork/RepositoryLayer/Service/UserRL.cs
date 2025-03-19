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

        // Get User by Email
        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Register User
        public async Task<UserEntity> RegisterUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Login User (Validate credentials)
        public async Task<UserEntity?> LoginUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null; // User not found

            // Verify the password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password.Trim(), user.PasswordHash);
            if (!isPasswordValid) return null; // Invalid password

            return user; // Return user if authenticated
        }

        // Update User
        public async Task UpdateUserAsync(UserEntity user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Save Password Reset Token
        public async Task SavePasswordResetTokenAsync(UserEntity user, string resetToken)
        {
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour
            await UpdateUserAsync(user);
        }

        // Get User by Reset Token
        public async Task<UserEntity?> GetUserByResetTokenAsync(string resetToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetToken == resetToken && u.ResetTokenExpiry > DateTime.UtcNow);
        }
    }
}
