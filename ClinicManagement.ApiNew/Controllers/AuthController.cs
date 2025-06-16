using ClinicManagement.ApiNew.DTOs.Auth;
using ClinicManagement.ApiNew.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger; // Added for logging
        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return BadRequest("Username, Password, and Email are required.");
            }

            try
            {
                var newUser = await _authService.Register(registerDto);
                if (newUser == null)
                {
                    return BadRequest("Username or Email already exists, or specified role is invalid.");
                }

                // Log the registration for auditing
                _logger.LogInformation("User registered: {Username} (ID: {UserId})", newUser.UserName, newUser.Id);

                // Optionally, log in the user immediately after registration and return a token
                var loginDto = new LoginDto { Username = registerDto.Username, Password = registerDto.Password };
                var token = await _authService.Login(loginDto);

                if (token == null)
                {
                    // This should ideally not happen if registration was successful
                    _logger.LogError("Failed to generate token for newly registered user: {Username}", registerDto.Username);
                    return StatusCode(StatusCodes.Status500InternalServerError, "User registered but failed to generate authentication token.");
                }

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", registerDto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during registration.");
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User login credentials.</param>
        /// <returns>A JWT token on successful login or an unauthorized error.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            try
            {
                var token = await _authService.Login(loginDto);

                if (token == null)
                {
                    return Unauthorized("Invalid username, password, or inactive account.");
                }

                _logger.LogInformation("User logged in: {Username}", loginDto.Username);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login for {Username}", loginDto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during login.");
            }
        }
    }
}
