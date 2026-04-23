using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.DTOs
{
    public class UpdateOrderStatusRequestDto
    {
        public OrderStatus Status { get; set; }
    }
}
