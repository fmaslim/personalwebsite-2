using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.DTOs.Common
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ServiceError> Errors { get; set; } = new();
        public List<FieldError> FieldErrors { get; set; } = new();
        public int StatusCode { get; set; }
    }
}
