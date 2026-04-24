using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.DTOs.Orders
{
    public class UpdateOrderStatusRequestDto
    {
        public OrderStatus Status { get; set; }
    }
}
