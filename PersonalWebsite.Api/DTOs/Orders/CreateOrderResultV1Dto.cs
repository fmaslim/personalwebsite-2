namespace PersonalWebsite.Api.DTOs.Orders
{
    public class CreateOrderResultV1Dto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public CreateOrderResponseV1Dto? Data { get; set; }
    }
}
