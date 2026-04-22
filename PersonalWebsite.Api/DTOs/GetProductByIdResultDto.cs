namespace PersonalWebsite.Api.DTOs
{
    public class GetProductByIdResultDto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProductDetailsDto? Data { get; set; }
    }
}
