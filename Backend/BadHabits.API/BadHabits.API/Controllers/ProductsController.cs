using BadHabits.API.Data;
using BadHabits.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadHabits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // --- HÀM 1: Lấy danh sách sản phẩm (Hỗ trợ lọc theo Danh mục) ---
        [HttpGet]
        public IActionResult GetProducts([FromQuery] int? categoryId)
        {
            var query = _context.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = query.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                price = p.Price,
                imageUrl = p.ImageUrl,
                categoryId = p.CategoryId
            }).ToList();

            return Ok(new { success = true, data = products });
        }

        // --- HÀM 2: Lấy chi tiết 1 sản phẩm ---
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    imageUrl = p.ImageUrl,
                    description = p.Description ?? "Chất liệu cotton 100%, form boxy fit hiện đại, phù hợp đi dạo phố."
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy sản phẩm này" });
            }

            return Ok(new { success = true, data = product });
        }
        // POST: api/Products
        [HttpPost]
        [Authorize] // Khoan hãy để (Roles = "Admin"), để bình thường test cho mượt đã
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu trống!" });
            }

            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return Ok(new { success = true, message = "Thêm sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi SQL: " + ex.Message });
            }
        }
    }
}