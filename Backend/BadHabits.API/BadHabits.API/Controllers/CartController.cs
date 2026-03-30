using System.Security.Claims;
using BadHabits.API.Data;
<<<<<<< HEAD
=======
using BadHabits.API.DTOs;
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
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

<<<<<<< HEAD
        // ĐÂY LÀ HÀM QUAN TRỌNG NHẤT VỪA BỊ MẤT: Dùng để kết nối Database
=======
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
        public CartController(AppDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
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
=======
        // POST: api/cart/add
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartDto request)
        {
            // 1. Soi Thẻ JWT để lấy ID của người dùng đang đăng nhập
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập lại!" });
            }
            int userId = int.Parse(userIdString);

<<<<<<< HEAD
            // Tự dò VariantId dựa vào ProductId và Size
            var variant = _context.Variants.FirstOrDefault(v => v.ProductId == request.ProductId && v.Size == request.Size);
            if (variant == null)
            {
                return BadRequest(new { success = false, message = "Sản phẩm hiện không có kích thước này!" });
            }

            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
=======
            // 2. Tìm xem ông này đã có cái giỏ hàng nào trong DB chưa?
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // Nếu chưa có giỏ (lần đầu mua hàng) -> Cấp cho 1 cái giỏ mới
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

<<<<<<< HEAD
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.VariantId == variant.Id);
            if (existingItem != null)
            {
=======
            // 3. Kiểm tra xem món hàng này (VariantId) đã có sẵn trong giỏ chưa?
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.VariantId == request.VariantId);

            if (existingItem != null)
            {
                // Nếu áo này có trong giỏ rồi -> Chỉ cần cộng dồn số lượng lên
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
                existingItem.Quantity += request.Quantity;
            }
            else
            {
<<<<<<< HEAD
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
=======
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
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
        }
    }
}