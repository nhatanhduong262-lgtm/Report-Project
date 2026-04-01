using BadHabits.API.Data;
using BadHabits.API.DTOs;
using BadHabits.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}