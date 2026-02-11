using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NET_MVC.Data;
using NET_MVC.Dto;
using NET_MVC.Helpers;
using NET_MVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NET_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Tìm user trong DB (bao gồm cả Role để tí nữa phân quyền)
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == request.Username);

            // 2. Kiểm tra user và mật khẩu
            if (user == null || user.Password != HashCodeHelper.GetMd5(request.Password))
            {
                return Unauthorized("Sai tài khoản hoặc mật khẩu!");
            }

            // 3. Nếu đúng, tạo Token JWT (Tôi sẽ viết hàm GenerateToken ở dưới)
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.Name
            });
        }

        private string GenerateJwtToken(Users user)
        {
            // 1. Tạo danh sách các "Claim" (Thông tin định danh)
            // Trong PHP bạn hay nhét vào mảng, ở đây dùng Claim
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.Name ?? string.Empty), // Phân quyền dựa trên cái này đây
        new Claim("UserId", user.Id.ToString())
    };

            // 2. Tạo Key bí mật (Lấy từ appsettings.json)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Thiết lập thông số Token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Hết hạn sau 1 ngày
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
