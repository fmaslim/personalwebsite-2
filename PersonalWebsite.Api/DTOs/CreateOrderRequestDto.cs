namespace PersonalWebsite.Api.DTOs
{
    public class CreateOrderRequestDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
