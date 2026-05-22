using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Auth;
using PersonalWebsite.Api.Extensions;
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
        public async Task<IActionResult> Login(LoginRequestV3Dto dto)
        {
            var result = await _authService.LoginV3Async(dto);

            return result.ToActionResult();
        }
    }
}
