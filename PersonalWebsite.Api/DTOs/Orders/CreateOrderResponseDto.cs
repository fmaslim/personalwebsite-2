namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderResponseDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
