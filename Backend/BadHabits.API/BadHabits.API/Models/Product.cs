namespace BadHabits.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Sản phẩm này thuộc danh mục nào
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Một sản phẩm có nhiều biến thể (Size S, M, L)
        public ICollection<Variant> Variants { get; set; } = new List<Variant>();
    }
}