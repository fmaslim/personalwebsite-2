namespace PersonalWebsite.Api.DTOs
{
    public class UpdateProductResultV2Dto
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public UpdateProductResponseV2Dto? Data { get; set; }
    }
}
