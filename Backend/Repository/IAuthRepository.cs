using Backend.Models.Domains;
using Backend.Models.DTO;

namespace Backend.Repository
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string username);
        Task<AuthenticationClass> Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
    }
}
