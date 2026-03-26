namespace BadHabits.API.Models
{
    public class Variant
    {
        public int Id { get; set; }

        // Biến thể này thuộc về sản phẩm nào
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string Size { get; set; } = string.Empty; // S, M, L, XL
        public string Color { get; set; } = string.Empty;

        // Số lượng tồn kho (Để biết khi nào hiện chữ "Hết hàng")
        public int StockQuantity { get; set; }
    }
}