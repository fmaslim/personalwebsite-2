namespace PersonalWebsite.Api.DTOs.Orders
{
    public class OrderSummaryResponseDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
