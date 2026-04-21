using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v2/auth")]
    public class AuthV2Controller : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            dto.Username = dto.Username.ToLower() + " this is from request dto"; // Normalize username to lowercase
            dto.Password = dto.Password.ToLower() + " this is from request dto"; // Normalize password to lowercase
            // Implement your login logic here
            return Ok(dto);
        }
    }
}
