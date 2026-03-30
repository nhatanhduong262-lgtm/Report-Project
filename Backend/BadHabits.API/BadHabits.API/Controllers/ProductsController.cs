using BadHabits.API.Data;
<<<<<<< HEAD
using BadHabits.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
=======
using BadHabits.API.DTOs;
using BadHabits.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3

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

<<<<<<< HEAD
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
=======
        // POST: api/products
        [HttpPost]
        public IActionResult CreateProduct([FromBody] CreateProductDto newProductDto)
        {
            // 1. Kiểm tra xem CategoryId có tồn tại trong Database không
            var categoryExists = _context.Categories.Any(c => c.Id == newProductDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { success = false, message = "Danh mục sản phẩm không tồn tại!" });
            }

            // 2. Chuyển đổi DTO thành Model Product thật
            var product = new Product
            {
                Name = newProductDto.Name,
                Description = newProductDto.Description,
                Price = newProductDto.Price,
                ImageUrl = newProductDto.ImageUrl,
                CategoryId = newProductDto.CategoryId
            };

            // 3. Thêm các biến thể vào sản phẩm
            foreach (var variantDto in newProductDto.Variants)
            {
                product.Variants.Add(new Variant
                {
                    Size = variantDto.Size,
                    Color = variantDto.Color,
                    StockQuantity = variantDto.StockQuantity
                });
            }

            // 4. Lưu tất cả xuống SQL Server
            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Thêm sản phẩm thành công!", data = product });
        }
        // GET: api/products
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            // Lấy danh sách sản phẩm, ĐỒNG THỜI lấy luôn cả Danh mục và Các biến thể (Size/Màu) của nó
            var products = _context.Products
                .Include(p => p.Category) // Nối bảng Category
                .Include(p => p.Variants) // Nối bảng Variant
                .ToList();

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách sản phẩm thành công",
                data = products
            });
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
        }
    }
}