using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Validation
{
    public static class PaginationValidation
    {
        public static void AddPaginationErrors(List<ServiceError> errors, int page, int pageSize, int maxPageSize = 50)
        {
            if(page <= 0)
            {
                errors.Add(new ServiceError
                {
                    Code = "InvalidPageNumber",
                    Message = "Page number must be greater than 0.",
                    Field = "page",
                    Type = ServiceErrorType.Validation
                });
            }
            if(pageSize <= 0)
            {
                errors.Add(new ServiceError
                {
                    Code = "InvalidPageSize",
                    Message = "Page size must be greater than 0.",
                    Field = "pageSize",
                    Type = ServiceErrorType.Validation
                });
            }
            if (pageSize > maxPageSize)
            {
                errors.Add(new ServiceError
                {
                    Code = "PageSizeTooLarge",
                    Message = $"Page size cannot be greater than {maxPageSize}.",
                    Field = "pageSize",
                    Type = ServiceErrorType.Validation
                });
            }
        }
    }
}
