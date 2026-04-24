namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderDetailRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
