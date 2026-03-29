using System.Security.Claims;
using BadHabits.API.Data;
using BadHabits.API.DTOs;
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

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartDto request)
        {
            // 1. Soi Thẻ JWT để lấy ID của người dùng đang đăng nhập
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập lại!" });
            }
            int userId = int.Parse(userIdString);

            // 2. Tìm xem ông này đã có cái giỏ hàng nào trong DB chưa?
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // Nếu chưa có giỏ (lần đầu mua hàng) -> Cấp cho 1 cái giỏ mới
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // 3. Kiểm tra xem món hàng này (VariantId) đã có sẵn trong giỏ chưa?
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.VariantId == request.VariantId);

            if (existingItem != null)
            {
                // Nếu áo này có trong giỏ rồi -> Chỉ cần cộng dồn số lượng lên
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // Nếu chưa có -> Tạo một món hàng mới bỏ vào giỏ
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    VariantId = request.VariantId,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(newItem);
            }

            // 4. Lưu tất cả thay đổi xuống SQL Server
            _context.SaveChanges();

            return Ok(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!" });
        }
        // GET: api/cart
        [HttpGet]
        public IActionResult GetMyCart()
        {
            // 1. "Soi" Thẻ JWT để biết user nào đang gọi
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập!" });
            }
            int userId = int.Parse(userIdString);

            // 2. Tìm cái giỏ của ông này
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);

            // Nếu không tìm thấy giỏ -> Giỏ hàng trống
            if (cart == null)
            {
                return Ok(new
                {
                    success = true,
                    message = "Giỏ hàng trống",
                    data = new { items = new List<object>(), totalCartPrice = 0 }
                });
            }

            // 3. Lấy các món hàng và "Join" (Nối) với bảng Variant, Product để lấy Tên, Ảnh, Giá
            var cartItemsData = _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .Join(_context.Variants.Include(v => v.Product), // Nối với bảng Variants (kèm theo Product)
                      ci => ci.VariantId,                        // Khóa ngoại ở bảng CartItem
                      v => v.Id,                                 // Khóa chính ở bảng Variant
                      (ci, v) => new                             // Cấu trúc lại dữ liệu trả về cho đẹp
                      {
                          cartItemId = ci.Id,
                          variantId = ci.VariantId,
                          productName = v.Product!.Name,         // Lấy tên từ bảng Product
                          imageUrl = v.Product.ImageUrl,
                          size = v.Size,                         // Lấy size từ bảng Variant
                          color = v.Color,
                          price = v.Product.Price,
                          quantity = ci.Quantity,
                          totalPrice = v.Product.Price * ci.Quantity // Tự động tính tiền món đó
                      }).ToList();

            // 4. Cộng dồn tổng tiền của toàn bộ giỏ hàng
            decimal cartTotal = cartItemsData.Sum(item => item.totalPrice);

            // 5. Trả kết quả về cho Frontend
            return Ok(new
            {
                success = true,
                data = new
                {
                    cartId = cart.Id,
                    items = cartItemsData,
                    totalCartPrice = cartTotal
                }
            });
        }
    }
}