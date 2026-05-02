namespace PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models.Errors;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0 && FieldErrors.Count == 0;

    public List<string> Errors { get; set; } = new();

    public List<PersonalWebsite.Api.Models.Errors.ServiceError> FieldErrors { get; set; } = new();

    public void AddFieldError(string field, string message)
    {
        FieldErrors.Add(new Models.Errors.ServiceError
        {
            Code = "ValidationError",
            Field = field,
            Message = message,
            Type = ServiceErrorType.Validation
        });
    }
}
