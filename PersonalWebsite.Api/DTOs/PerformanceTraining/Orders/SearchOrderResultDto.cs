namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Orders
{
    public class SearchOrderResultDto
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public int SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; } = string.Empty;
        public decimal TotalDue { get; set; }
        public int ItemCount { get; set; }
    }
}
