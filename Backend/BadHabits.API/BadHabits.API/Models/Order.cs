namespace BadHabits.API.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Liên kết với người dùng nào đã đặt hàng
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }

        // Thông tin giao hàng
        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Trạng thái: Pending (Chờ xử lý), Shipping (Đang giao), Completed (Hoàn thành), Canceled (Đã hủy)
        public string Status { get; set; } = "Pending";

        // Một đơn hàng có thể có nhiều chi tiết (nhiều sản phẩm)
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}