namespace PersonalWebsite.Api.DTOs
{
    public class CreateOrderRequestV1Dto
    {
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
