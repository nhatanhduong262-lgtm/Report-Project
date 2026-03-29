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
    [Authorize] // Phải đăng nhập mới được đặt hàng!
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public IActionResult Checkout([FromBody] CheckoutRequestDto request)
        {
            // 1. Soi Thẻ JWT lấy ID người dùng
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Lấy giỏ hàng hiện tại của người dùng này
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // Nếu giỏ hàng trống không thì báo lỗi, không cho đặt
            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest(new { success = false, message = "Giỏ hàng của bạn đang trống, hãy chọn mua gì đó trước nhé!" });
            }

            // 3. Tính tổng tiền và chuẩn bị danh sách Chi tiết đơn hàng
            decimal totalPrice = 0;
            var orderDetailsList = new List<OrderDetail>();

            foreach (var cartItem in cart.CartItems)
            {
                // Phải lấy giá gốc từ Database ra tính, KHÔNG ĐƯỢC tin tưởng giá do Frontend gửi lên (chống hack)
                var variant = _context.Variants
                    .Include(v => v.Product)
                    .FirstOrDefault(v => v.Id == cartItem.VariantId);

                if (variant != null && variant.Product != null)
                {
                    decimal unitPrice = variant.Product.Price;
                    totalPrice += unitPrice * cartItem.Quantity;

                    // Tạo chi tiết hóa đơn cho món hàng này
                    orderDetailsList.Add(new OrderDetail
                    {
                        VariantId = cartItem.VariantId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrice // Lưu lại giá lúc mua, lỡ mai mốt áo tăng giá thì hóa đơn cũ không đổi
                    });
                }
            }

            // 4. Tạo Hóa đơn mẹ (Order)
            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = request.ShippingAddress,
                PhoneNumber = request.PhoneNumber,
                TotalPrice = totalPrice,
                Status = "Pending", // Trạng thái: Đang chờ xử lý
                OrderDetails = orderDetailsList // Nhét danh sách hóa đơn con vào đây
            };

            _context.Orders.Add(newOrder);

            // 5. RẤT QUAN TRỌNG: Đặt hàng xong thì phải dọn sạch giỏ hàng!
            _context.CartItems.RemoveRange(cart.CartItems);

            // 6. Ra lệnh cho SQL Server thực thi tất cả những việc trên CÙNG MỘT LÚC
            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "Tuyệt vời! Bạn đã đặt hàng thành công.",
                orderId = newOrder.Id
            });
        }
    }
}