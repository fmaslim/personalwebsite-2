using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Sunday, 04/16/2026 - hardoded fake orders for training purposes. In a real application, you would query the database for the user's orders.
        private static readonly List<Order> _orders = new()
        {
            new Order { Id = 1, UserId = 1, ProductName = "Laptop", TotalAmount = 1200 },
            new Order { Id = 2, UserId = 2, ProductName = "Phone", TotalAmount = 800 },
            new Order { Id = 3, UserId = 1, ProductName = "Keyboard", TotalAmount = 100 }
        };

        private readonly AdventureWorksContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AdventureWorksContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            var user = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            if (user.PasswordHash != dto.Password)
            {
                return Unauthorized("Invalid username or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new LoginResponseDto
            {
                Token = tokenString,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            });
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult Secret()
        {
            return Ok("This is a secret endpoint!");
        }

        // This endpoint reads the claims from the JWT token and returns the username and role of the authenticated user
        [Authorize]
        [HttpGet("claims")]
        public IActionResult Me()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var jti = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            return Ok(new { Username = username, Role = role, Jti = jti });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("You are an admin.");
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager")]
        public IActionResult Manager()
        {
            return Ok("You are a manager.");
        }

        [Authorize]
        [HttpGet("whoami")]
        public IActionResult Whoami()
        {
            return Ok(new
            {
                UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                IsAdmin = User.IsInRole("Admin"),
                IsManager = User.IsInRole("Manager")
            });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("manage-content")]
        public IActionResult ManageContent()
        {
            return Ok("You are either an admin or manager. You canb manage content.");
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User id claim is missing.");
            }
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user id claim.");
            }
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            if (order.UserId != userId) // ownership check - only the user who owns the order can access it
            {
                // return Forbid("You are not authorized to access this order.");
                return Forbid();
            }
            return Ok(order);
        }

        [Authorize]
        [HttpGet("my-orders")]
        public IActionResult GetMyOrders()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User id claim is missing.");
            }
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user id claim.");
            }
            var orders = _orders.Where(o => o.UserId == userId).ToList();
            return Ok(orders);
        }
    }
}
