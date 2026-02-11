using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NET_MVC.Constants;
using NET_MVC.Data;
using NET_MVC.Dto;
using NET_MVC.Helpers;
using NET_MVC.Interface;
using NET_MVC.Models;
using System.Net;

namespace NET_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult Get(int page = 1, int pageSize = 10)
        {
            // 1. Tính toán số bản ghi bỏ qua (Phân trang)
            int skip = (page - 1) * pageSize;

            // 2. Truy vấn dữ liệu
            var query = _context.Users
                .Include(u => u.Role) // JOIN sang bảng Role để lấy Name
                .AsQueryable();

            // 3. Tổng số lượng để trả về cho Client biết đường làm UI phân trang
            var totalItems = query.Count();

            // 4. Lấy dữ liệu trang hiện tại và map vào ViewModel
            var users = query
                .Skip(skip)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    Permission = u.Role.RolePermissions.Select(rp => new PermissionViewModel
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name
                    }).ToList()
                })
                .ToList();

            return Ok(new
            {
                Total = totalItems,
                Page = page,
                PageSize = pageSize,
                Data = users
            });
        }

        [HttpPost("create")]
        [Authorize]
        public IActionResult Create([FromBody] UserCreateDto model)
        {
            // 1. Kiểm tra xem username đã tồn tại chưa
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                return BadRequest(new ApiResponse<object>(false, 400, "Username đã tồn tại", null));
            }

            // 2. Tạo User mới và băm mật khẩu (Dùng thư viện BCrypt hoặc tương đương)
            var newUser = new Users
            {
                Username = model.Username,
                Password = HashCodeHelper.ToMd5(model.Password),
                RoleId = 2
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new ApiResponse<object>(true, 200, "Thêm thành công", newUser.Id));
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public IActionResult Update(int id, [FromBody] UserUpdateDto model)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound(new ApiResponse<object>(false, 404, "Không tìm thấy User", null));

            // CHẶN CRASH: Kiểm tra RoleId có tồn tại trong bảng Roles không
            var roleExists = _context.Roles.Any(r => r.Id == model.RoleId);
            if (!roleExists)
            {
                return BadRequest(new ApiResponse<object>(false, 400, $"RoleId {model.RoleId} không tồn tại trong hệ thống!", null));
            }

            user.Username = model.Username;
            user.RoleId = model.RoleId;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = HashCodeHelper.ToMd5(model.Password);
            }

            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return Ok(new ApiResponse<object>(true, 200, "Cập nhật thành công", null));
            }
            catch (Exception ex)
            {
                // Backup plan: Nếu vẫn có lỗi DB bất ngờ thì trả về 500 chứ không crack app
                return StatusCode(500, new ApiResponse<object>(false, 500, "Lỗi hệ thống khi lưu DB", ex.Message));
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = RoleName.Admin)] // <-- Khóa bảo mật: Chỉ Admin mới vào được đây
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy User" });

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(new { message = "Đã xóa User thành công!" });
        }
    }
}
