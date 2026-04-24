namespace PersonalWebsite.Api.DTOs.Orders
{
    public class GetOrderByIdResponseDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public List<OrderDetailResponseDto> Items { get; set; } = new();
    }
}
