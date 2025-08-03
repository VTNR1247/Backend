using Backend.Models.Domains;
using Backend.Models.DTO;
using Backend.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (await _authRepository.UserExists(registerDTO.Username))
                return BadRequest(new { message = "User Already existed" });
            var user = _authRepository.Register(registerDTO);
            return Ok(new { message = "User Registered Successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var token = await _authRepository.Login(loginDTO);
            if (token == null)
                return Unauthorized(new {message = "Invalid Username or password."});
            return Ok(new { token });
        }
    }
}
