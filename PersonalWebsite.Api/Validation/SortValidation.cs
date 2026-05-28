using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Validation
{
    public static class SortValidation
    {
        public static void AddSortErrors(List<ServiceError> errors, string? sortBy, string? sortDir, string[] allowedSortBy, string[] allowedSortDir)
        {
            if (!string.IsNullOrWhiteSpace(sortBy) && !allowedSortBy.Contains(sortBy.Trim().ToLower()))
            {
                errors.Add(new ServiceError
                {
                    Code = "InvalidSortBy",
                    Message = $"Invalid sortBy value. Allowed values are: {string.Join(", ", allowedSortBy)}.",
                    Field = "sortBy",
                    Type = ServiceErrorType.Validation
                });
            }
            if(!string.IsNullOrWhiteSpace(sortDir) && !allowedSortDir.Contains(sortDir))
            {
                errors.Add(new ServiceError
                {
                    Code = "InvalidSortDir",
                    Message = $"Invalid sortDir value. Allowed values are: {string.Join(", ", allowedSortDir)}.",
                    Field = "sortDir",
                    Type = ServiceErrorType.Validation
                });
            }
        }


    }
}
