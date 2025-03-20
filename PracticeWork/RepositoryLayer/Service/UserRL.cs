using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookDBContext _context;

        public UserRL(AddressBookDBContext context)
        {
            _context = context;
        }

        // Register a new user
        public async Task<UserEntity> RegisterUser(UserEntity user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Get user by email
        public async Task<UserEntity> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Update user details
        public async Task UpdateUser(UserEntity user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserEntity> GetUserByResetToken(string resetToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken);
        }
    }
}
