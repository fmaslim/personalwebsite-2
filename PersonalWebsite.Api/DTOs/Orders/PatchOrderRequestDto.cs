namespace PersonalWebsite.Api.DTOs.Orders
{
    public class PatchOrderRequestDto
    {
        public string? ProductName { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
