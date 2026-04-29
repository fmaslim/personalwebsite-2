namespace PersonalWebsite.Api.DTOs.Orders
{
    public class OrderSearchRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
