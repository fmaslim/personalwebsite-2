namespace PersonalWebsite.Api.DTOs.Orders
{
    public class OrderPaymentResponseDto
    {
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
