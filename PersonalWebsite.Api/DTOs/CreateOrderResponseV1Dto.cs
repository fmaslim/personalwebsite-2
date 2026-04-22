namespace PersonalWebsite.Api.DTOs
{
    public class CreateOrderResponseV1Dto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
