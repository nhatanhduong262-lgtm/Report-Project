namespace BadHabits.API.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Giỏ hàng này của ai?
        public int UserId { get; set; }
        public User? User { get; set; }

        // Chứa những món đồ nào?
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}