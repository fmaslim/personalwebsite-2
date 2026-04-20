namespace PersonalWebsite.Api.DTOs
{
    public class PatchOrderRequestDto
    {
        public string? ProductName { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
