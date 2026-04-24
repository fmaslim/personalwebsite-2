using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.DTOs
{
    public class OrderQueryParamsDto
    {
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "orderDate";
        public string SortDir { get; set; } = "desc";
    }
}
