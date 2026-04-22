namespace PersonalWebsite.Api.DTOs
{
    public class CreateProductResultV2Dto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public CreateProductResponseV2Dto? Data { get; set; }
    }
}
