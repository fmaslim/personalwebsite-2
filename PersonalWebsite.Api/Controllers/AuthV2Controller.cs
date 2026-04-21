using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v2/auth")]
    public class AuthV2Controller : ControllerBase
    {
        [HttpPost("login")]
        public ActionResult<LoginResponseV2Dto> Login(LoginRequestDto dto)
        {
            var response = new LoginResponseV2Dto
            {
                Username = dto.Username,
                Message = "Login successful from v2",
                Version = "v2",
                ExpiresIn = 3600,
                Token = "fake-jwt-token-v2",
                RefreshToken = "fake-refresh-token-v2"
            };
            // Implement your login logic here
            return Ok(response);
        }

        //[HttpPost("login")]
        //public IActionResult Login(LoginRequestDto dto)
        //{
        //    var response = new LoginResponseV2Dto
        //    {
        //        Username = dto.Username,
        //        Message = "Login successful",
        //        Version = "v2",
        //        ExpiresIn = 3600
        //    };
        //    // Implement your login logic here
        //    return Ok(response);
        //}
    }
}
