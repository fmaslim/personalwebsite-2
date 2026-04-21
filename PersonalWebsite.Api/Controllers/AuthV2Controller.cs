using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v2/auth")]
    public class AuthV2Controller : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthV2Controller(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginErrorResponseV2Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginErrorResponseV2Dto), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseV2Dto>> Login(LoginRequestDto dto)
        {
            // Tuesday, 04/21/2026 - refactored to service layer
            var result = await _authService.LoginV2Async(dto);
            if (result.StatusCode == 400)
            {
                var errorResponse = new LoginErrorResponseV2Dto
                {
                    Message = result.Message
                };
                return BadRequest(errorResponse);
            }
            if (result.StatusCode == 401)
            {
                var errorResponse = new LoginErrorResponseV2Dto
                {
                    Message = result.Message
                };
                return Unauthorized(errorResponse);
            }
            // Implement your login logic here
            return Ok(result.Data);
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
