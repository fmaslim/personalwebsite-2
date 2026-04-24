namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderRequestV2Dto
    {
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public List<CreateOrderItemRequestV2Dto> Items { get; set; } = new();
    }
}
