namespace PersonalWebsite.Api.DTOs.Orders
{
    public class UpdateOrderRequestDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
