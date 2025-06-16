// Location: ClinicManagement.ApiNew/Services/AuthService.cs
using ClinicManagement.ApiNew.DTOs.Auth;
using ClinicManagement.Data.Context;
using ClinicManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicManagement.ApiNew.Services
{
    /// <summary>
    /// Service for handling authentication-related operations like user registration, login,
    /// password hashing, and JWT token generation.
    /// </summary>
    public class AuthService
    {
        private readonly ClinicManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ClinicManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto">Registration details.</param>
        /// <returns>The created User object or null if registration fails (e.g., username/email taken).</returns>
        public async Task<User?> Register(RegisterUserDto registerDto)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.UserName == registerDto.Username))
            {
                return null; // Username already exists
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null; // Email already exists
            }

            // Determine the role for the new user
            Role? role;
            if (string.IsNullOrEmpty(registerDto.RoleName))
            {
                // Default to 'Patient' role if no role is specified (e.g., for public registration)
                role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
                if (role == null)
                {
                    // If 'Patient' role doesn't exist, create it. This is a basic setup.
                    // In a real app, roles should be pre-seeded.
                    role = new Role { Name = "Patient", CreatedAt = DateTime.UtcNow, NormalizedName = "PATIENT" }; // Add NormalizedName
                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Try to find the specified role
                role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == registerDto.RoleName);
                if (role == null)
                {
                    return null; // Specified role does not exist
                }
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var newUser = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true // IdentityUser requires this
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token upon successful login.
        /// </summary>
        /// <param name="loginDto">Login credentials.</param>
        /// <returns>A JWT token string or null if authentication fails.</returns>
        public async Task<string?> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                                    .Include(u => u.Role) // Eager load the Role to get RoleName
                                    .FirstOrDefaultAsync(u => u.UserName == loginDto.Username && u.IsActive == true);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // User not found, inactive, or invalid credentials
            }

            return GenerateJwtToken(user);
        }

        /// <summary>
        /// Generates a JSON Web Token (JWT) for a given user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The JWT token string.</returns>
        private string GenerateJwtToken(User user)
        {
            // Get JWT settings from configuration
            var jwtSecret = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expirationDays = Convert.ToDouble(_configuration["Jwt:DurationInDays"]);

            if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration is missing in appsettings.json.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // FIX: Add null-coalescing
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),   // FIX: Add null-coalescing
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Unknown")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(expirationDays),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}