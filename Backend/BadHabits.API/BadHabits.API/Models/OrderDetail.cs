namespace BadHabits.API.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // Nằm trong đơn hàng nào
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // Mua biến thể sản phẩm nào (Áo thun đen size L)
        // Lưu ý: Mình giả định bạn đã có class Variant, nếu bạn gọi nó khác đi hãy sửa lại nhé
        public int VariantId { get; set; }
        // public Variant? Variant { get; set; } 

        public int Quantity { get; set; }

        // Giá tại thời điểm mua (để lỡ sau này áo tăng giá thì hóa đơn cũ không bị đổi)
        public decimal UnitPrice { get; set; }
    }
}