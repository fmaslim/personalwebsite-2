namespace PersonalWebsite.Api.DTOs
{
    public class CreateOrderResponseV3Dto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
