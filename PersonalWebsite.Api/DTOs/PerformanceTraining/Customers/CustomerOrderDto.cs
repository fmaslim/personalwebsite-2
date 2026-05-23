namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Customers
{
    public class CustomerOrderDto
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
