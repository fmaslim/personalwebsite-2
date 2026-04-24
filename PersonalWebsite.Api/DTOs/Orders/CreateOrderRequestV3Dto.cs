namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderRequestV3Dto
    {
        public int UserId { get; set; }
        public List<CreateOrderDetailRequestDto> Items { get; set; } = new();
    }
}
