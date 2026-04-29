namespace PersonalWebsite.Api.DTOs.Orders
{
    public class OrderSearchResultDto
    {
        public int SalesOrderId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public decimal? TotalDue { get; set; }
        public int ItemCount { get; set; }
    }
}
