using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Service
{
    public class AuthBL : IAuthBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthBL> _logger;

        public AuthBL(IUserRL userRL, IConfiguration configuration, ILogger<AuthBL> logger)
        {
            _userRL = userRL ?? throw new ArgumentNullException(nameof(userRL));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Registers a new user
        public async Task<string?> RegisterUserAsync(UserRegisterDTO userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            try
            {
                // Check if the user already exists
                var existingUser = await _userRL.GetUserByEmailAsync(userDto.Email);
                if (existingUser != null) return "User already exists.";

                // Hash the password securely
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                // Create a new user entity
                var newUser = new UserEntity
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    PasswordHash = passwordHash
                };

                // Save the user to the database
                await _userRL.RegisterUserAsync(newUser);

                // Generate and return JWT token
                return GenerateJwtToken(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                throw new InvalidOperationException("Registration failed.");
            }
        }

        // Authenticates a user
        public async Task<string?> LoginUserAsync(UserLoginDTO loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            try
            {
                var user = await _userRL.GetUserByEmailAsync(loginDto.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid credentials for email: {Email}", loginDto.Email);
                    return null; // Invalid credentials
                }

                return GenerateJwtToken(user); // Generate JWT token
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login.");
                throw new InvalidOperationException("Login failed.");
            }
        }

        // Generates a JWT token for the authenticated user
        private string GenerateJwtToken(UserEntity user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Id == null || IsInvalidUserId(user.Id)) throw new ArgumentException("Invalid user ID.");

            try
            {
                var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
                var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
                var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token.");
                throw new InvalidOperationException("Token generation failed.");
            }
        }

        // Helper method to validate the user ID
        private bool IsInvalidUserId(object userId)
        {
            return userId switch
            {
                Guid guid => guid == Guid.Empty,
                int id => id <= 0,
                _ => true
            };
        }

        // Initiates password reset
        public async Task<string> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

            try
            {
                var user = await _userRL.GetUserByEmailAsync(email);
                if (user == null) throw new InvalidOperationException("User not found.");

                // Generate a secure reset token
                string resetToken = Guid.NewGuid().ToString();
                await _userRL.SavePasswordResetTokenAsync(user, resetToken);

                // Normally, you'd send this token via email
                _logger.LogInformation("Generated password reset token for user: {Email}", email);

                return resetToken; // Return token for debugging (replace with email sending)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset.");
                throw new InvalidOperationException("Password reset failed.");
            }
        }

        // Resets user password
        public async Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
        {
            if (string.IsNullOrEmpty(resetToken) || string.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException("Token and password cannot be null.");

            try
            {
                var user = await _userRL.GetUserByResetTokenAsync(resetToken);
                if (user == null || user.ResetTokenExpiry < DateTime.UtcNow) return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.ResetToken = null;
                user.ResetTokenExpiry = null;

                await _userRL.UpdateUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset.");
                throw new InvalidOperationException("Password reset failed.");
            }
        }
    }
}
