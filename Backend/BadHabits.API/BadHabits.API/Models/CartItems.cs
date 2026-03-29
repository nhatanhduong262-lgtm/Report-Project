namespace BadHabits.API.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // Nằm trong giỏ hàng nào?
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // Khách chọn mua biến thể nào (ví dụ: Áo thun size L màu Đen)
        public int VariantId { get; set; }
        // Lưu ý: Tạm thời không gán public Variant Variant để tránh lỗi vòng lặp dữ liệu sau này

        // Số lượng mua là bao nhiêu?
        public int Quantity { get; set; }
    }
}