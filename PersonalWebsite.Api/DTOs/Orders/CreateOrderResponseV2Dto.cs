namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderResponseV2Dto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
