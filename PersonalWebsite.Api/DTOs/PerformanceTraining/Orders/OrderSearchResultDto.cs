namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Orders
{
    public class OrderSearchResultDto
    {
        public int OrderId { get; set; }

        public string? CustomerName { get; set; }

        public string? Status { get; set; }

        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
