namespace PersonalWebsite.Api.DTOs.Common
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0 && FieldErrors.Count == 0;

        public List<string> Errors { get; set; } = new();

        public List<ServiceError> FieldErrors { get; set; } = new();
    }
}
