namespace BadHabits.API.DTOs
{
    public class CheckoutRequestDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}