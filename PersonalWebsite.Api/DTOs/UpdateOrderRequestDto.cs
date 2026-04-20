namespace PersonalWebsite.Api.DTOs
{
    public class UpdateOrderRequestDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
