using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Auth;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v3/auth")]
    public class AuthV3Controller : Controller
    {
        private readonly IAuthService _authService;
        public AuthV3Controller(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseV3Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginErrorResponseV3Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginErrorResponseV3Dto), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseV3Dto>> Login(LoginRequestV3Dto dto)
        {
            var result = await _authService.LoginV3Async(dto);
            
            if (result.StatusCode == 400)
            {
                return BadRequest(new LoginErrorResponseV3Dto() { Message = result.Message });
            }
            if(result.StatusCode == 401)
            {
                return Unauthorized(new LoginErrorResponseV3Dto() { Message = result.Message });
            }

            return Ok(result.Data);
        }
    }
}
