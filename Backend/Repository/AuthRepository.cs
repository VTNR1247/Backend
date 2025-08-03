using Backend.Data;
using Backend.Models.Domains;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Repository
{
    public class AuthRepository: IAuthRepository
    {
        private readonly AuthDBContext _authDBContext;
        private readonly IConfiguration _configuration;

        public AuthRepository(AuthDBContext authDBContext, IConfiguration configuration)
        {
            _authDBContext = authDBContext;
            _configuration = configuration;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _authDBContext.Users.AnyAsync(u=>u.UserName == username);
        }

        public async Task<AuthenticationClass> Register(RegisterDTO dto)
        {
            var user = new AuthenticationClass
            {
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _authDBContext.Add(user);
            await _authDBContext.SaveChangesAsync();
            return user;
        }

        public async Task<string> Login(LoginDTO dto)
        {
            var user = await _authDBContext.Users.FirstOrDefaultAsync(u => u.UserName == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return null;
            }
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(AuthenticationClass user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Email, user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
             return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
