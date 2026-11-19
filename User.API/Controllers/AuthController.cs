using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using User.Data.Repository;
using UserEntity = User.Data.Entity.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.API.DTO;

namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null || string.IsNullOrEmpty(user.Password))
                return Unauthorized("Invalid username or password");

            PasswordVerificationResult passwordResult;

            try
            {
                passwordResult = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.Password,
                    dto.Password
                );
            }
            catch (FormatException)
            {
             
                return Unauthorized("Invalid username or password. Please register again.");
            }

            if (passwordResult == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid username or password");

          
            if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, dto.Password);
                await _userRepository.UpdateAsync(user);
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpirationInMinutes"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return Ok(new LoginResponceDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires
            });
        }
    }
}
