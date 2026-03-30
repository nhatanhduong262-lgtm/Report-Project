using BadHabits.API.Data;
using BadHabits.API.Models;
using Microsoft.AspNetCore.Mvc;
using BadHabits.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
namespace BadHabits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto request)
        {
            // 1. Kiểm tra Email trùng
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { success = false, message = "Email này đã được sử dụng!" });
            }

            // 2. Tạo User mới từ DTO gửi lên
            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                // Băm mật khẩu gốc thành chuỗi bảo mật
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Customer", // Ép cứng quyền mặc định
                CreatedAt = DateTime.Now
            };

            // 3. Lưu xuống Database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // 4. Trả về kết quả (Che mật khẩu đi)
            newUser.PasswordHash = "";
            return Ok(new { success = true, message = "Đăng ký thành công!", data = newUser });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            // 1. Tìm user theo Email
            var user = _context.Users.SingleOrDefault(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return BadRequest(new { success = false, message = "Email hoặc mật khẩu không đúng!" });
            }

<<<<<<< HEAD
            // 2. Kiểm tra mật khẩu (So sánh mật khẩu người dùng nhập với chuỗi băm trong DB)
            // Lưu ý: Đổi loginRequest.PasswordHash thành loginRequest.Password cho khớp với DTO
            bool isPasswordValid = false;
            try
            {
=======
            // 2. Kiểm tra mật khẩu (Bọc thép bằng Try-Catch)
            bool isPasswordValid = false;
            try
            {
                // BCrypt sẽ thử kiểm tra mật khẩu. Nếu dữ liệu trong DB bị rác/ngắn/hỏng, nó sẽ nhảy ngay xuống catch
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
                isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
            }
            catch
            {
<<<<<<< HEAD
                return BadRequest(new { success = false, message = "Dữ liệu tài khoản lỗi. Vui lòng tạo tài khoản mới!" });
            }

=======
                // Trả về lỗi đàng hoàng cho giao diện thay vì sập Server
                return BadRequest(new { success = false, message = "Dữ liệu tài khoản này bị lỗi (hệ thống cũ). Vui lòng đăng ký tài khoản mới!" });
            }

            // Nếu xác thực thành công nhưng mật khẩu sai
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
            if (!isPasswordValid)
            {
                return BadRequest(new { success = false, message = "Email hoặc mật khẩu không đúng!" });
            }

            // --- BẮT ĐẦU TẠO TOKEN ---
            var tokenHandler = new JwtSecurityTokenHandler();
<<<<<<< HEAD
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
=======
            var key = Encoding.UTF8.GetBytes("Jwt:Key");
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3

            // Nhét thông tin của user vào thẻ (Tên, Email, Quyền hạn)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role) // Rất quan trọng để phân biệt Admin/Customer sau này
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Thẻ có hạn dùng 7 ngày
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);
            // --- KẾT THÚC TẠO TOKEN ---

            // Trả về Token cho Frontend cất giữ
            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                token = jwtString // Trả thêm dòng này
            });
        }

        // GET: api/Auth/users
        [HttpGet("users")]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
    }
}