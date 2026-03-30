using System.Security.Claims;
using BadHabits.API.Data;
using BadHabits.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadHabits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // BẮT BUỘC: Phải có Thẻ JWT (đã đăng nhập) mới được vào đây
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        // ĐÂY LÀ HÀM QUAN TRỌNG NHẤT VỪA BỊ MẤT: Dùng để kết nối Database
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // --- Class DTO hứng dữ liệu từ Frontend ---
        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public string Size { get; set; }
            public int Quantity { get; set; }
        }

        // --- 1. POST: api/cart/add (Thêm vào giỏ) ---
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập lại!" });
            }
            int userId = int.Parse(userIdString);

            // Tự dò VariantId dựa vào ProductId và Size
            var variant = _context.Variants.FirstOrDefault(v => v.ProductId == request.ProductId && v.Size == request.Size);
            if (variant == null)
            {
                return BadRequest(new { success = false, message = "Sản phẩm hiện không có kích thước này!" });
            }

            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.VariantId == variant.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    VariantId = variant.Id,
                    Quantity = request.Quantity
                });
            }

            _context.SaveChanges();
            return Ok(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!" });
        }

        // --- 2. GET: api/cart (Lấy giỏ hàng) ---
        [HttpGet]
        public IActionResult GetMyCart()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized(new { success = false, message = "Vui lòng đăng nhập!" });
            int userId = int.Parse(userIdString);

            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                return Ok(new { success = true, message = "Giỏ hàng trống", data = new { items = new List<object>(), totalCartPrice = 0 } });
            }

            var cartItemsData = _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .Join(_context.Variants.Include(v => v.Product),
                      ci => ci.VariantId,
                      v => v.Id,
                      (ci, v) => new
                      {
                          cartItemId = ci.Id,
                          variantId = ci.VariantId,
                          productName = v.Product!.Name,
                          imageUrl = v.Product.ImageUrl,
                          size = v.Size,
                          color = v.Color,
                          price = v.Product.Price,
                          quantity = ci.Quantity,
                          totalPrice = v.Product.Price * ci.Quantity
                      }).ToList();

            decimal cartTotal = cartItemsData.Sum(item => item.totalPrice);

            return Ok(new { success = true, data = new { cartId = cart.Id, items = cartItemsData, totalCartPrice = cartTotal } });
        }

        // --- 3. PUT: api/cart/update/5?quantity=2 ---
        [HttpPut("update/{cartItemId}")]
        public IActionResult UpdateQuantity(int cartItemId, [FromQuery] int quantity)
        {
            var item = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null) return NotFound(new { success = false, message = "Không tìm thấy sản phẩm" });

            if (quantity <= 0) _context.CartItems.Remove(item);
            else item.Quantity = quantity;

            _context.SaveChanges();
            return Ok(new { success = true, message = "Đã cập nhật số lượng" });
        }

        // --- 4. DELETE: api/cart/remove/5 ---
        [HttpDelete("remove/{cartItemId}")]
        public IActionResult RemoveItem(int cartItemId)
        {
            var item = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null) return NotFound(new { success = false, message = "Không tìm thấy sản phẩm" });

            _context.CartItems.Remove(item);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ" });
        }
    }
}