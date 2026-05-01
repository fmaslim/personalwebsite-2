namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Customers
{
    public class CustomerOrderSummaryResultDto
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}
