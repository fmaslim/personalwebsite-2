namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Orders
{
    public class SearchOrderResultDto
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
