namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderItemRequestV2Dto
    {
        public int ProductId { get; set; }
        public short Quantity { get; set; }
    }
}
