namespace PersonalWebsite.Api.DTOs.Orders
{
    public class UpdateOrderStatusResponseDto
    {
        public int OrderId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
    }
}
