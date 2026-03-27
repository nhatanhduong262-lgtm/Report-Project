namespace BadHabits.API.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Chỉ cần truyền ID của danh mục (ví dụ: 1 là Áo thun, 2 là Quần)
        public int CategoryId { get; set; }

        // Một danh sách các biến thể (ví dụ: Size S màu đen, Size M màu trắng)
        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
    }

    public class CreateVariantDto
    {
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}