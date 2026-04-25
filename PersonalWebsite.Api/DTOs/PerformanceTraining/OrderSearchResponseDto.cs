namespace PersonalWebsite.Api.DTOs.PerformanceTraining
{
    public class OrderSearchResponseDto
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public int Status { get; set; }

        // public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
