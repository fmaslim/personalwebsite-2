namespace PersonalWebsite.Api.DTOs.Common
{
    public class ServiceError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Field { get; set; }
    }
}
