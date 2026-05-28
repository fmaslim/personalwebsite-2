namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderRequestDto
    {
        //public string ProductName { get; set; } = string.Empty;
        //public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
