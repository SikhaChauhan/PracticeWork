using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using RepositoryLayer.Repository;

namespace BusinessLayer.Service
{
    public class AuthService : IAuthBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRL userRL, IConfiguration configuration)
        {
            _userRL = userRL;
            _configuration = configuration;
        }

        public async Task<string?> RegisterUserAsync(UserRegisterDTO userDto)
        {
            var existingUser = await _userRL.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null) return "User already exists.";

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var newUser = new UserEntity
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = passwordHash
            };

            await _userRL.RegisterUserAsync(newUser);
            return GenerateJwtToken(newUser);
        }

        public async Task<string?> LoginUserAsync(UserLoginDTO loginDto)
        {
            var user = await _userRL.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null; // Invalid credentials

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
