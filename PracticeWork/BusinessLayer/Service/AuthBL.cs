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

namespace BusinessLayer.Service
{
    public class AuthBL : IAuthBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _configuration;

        public AuthBL(IUserRL userRL, IConfiguration configuration)
        {
            _userRL = userRL ?? throw new ArgumentNullException(nameof(userRL));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Registers a new user
        public async Task<string?> RegisterUserAsync(UserRegisterDTO userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

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

        // Authenticates a user
        public async Task<string?> LoginUserAsync(UserLoginDTO loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            Console.WriteLine($"Login attempt for: {loginDto.Email}");

            // Authenticate user using repository method
            var user = await _userRL.LoginUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                Console.WriteLine("Invalid email or password.");
                return null; // Invalid credentials
            }

            Console.WriteLine($"Login successful for: {user.Email}");

            // Generate and return JWT token
            return GenerateJwtToken(user);
        }


        // Generates a JWT token for the authenticated user
        private string GenerateJwtToken(UserEntity user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Validate user ID
            if (user.Id == null || IsInvalidUserId(user.Id))
            {
                throw new ArgumentException("Invalid user ID.");
            }

            // Retrieve JWT settings from configuration
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            // Create the security key and signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper method to validate the user ID
        private bool IsInvalidUserId(object userId)
        {
            return userId switch
            {
                Guid guid => guid == Guid.Empty,
                int id => id == 0,
                _ => true // If userId is neither int nor Guid, treat it as invalid
            };
        }
    }
}
