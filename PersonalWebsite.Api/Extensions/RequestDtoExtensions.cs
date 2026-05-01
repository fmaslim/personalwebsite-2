using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using System.Runtime.CompilerServices;

namespace PersonalWebsite.Api.Extensions
{
    public static class RequestDtoExtensions
    {
        public static void Normalize(this CustomerOrderSummaryRequestDto requestDto)
        {
            if (requestDto.PageNumber <= 0)
            {
                requestDto.PageNumber = 1;
            }
            if (requestDto.PageSize <= 0)
            {
                requestDto.PageSize = 10;
            }
            if (requestDto.PageSize > 100)
            {
                requestDto.PageSize = 100;
            }
            if (requestDto.MinOrderCount < 0)
            {
                requestDto.MinOrderCount = 0;
            }
            if (requestDto.MinTotalSpent < 0)
            {
                requestDto.MinTotalSpent = 0;
            }
            var allowedSortBy = new[] { "customerName", "orderCount", "totalSpent" };
            if (string.IsNullOrWhiteSpace(requestDto.SortBy) || !allowedSortBy.Contains(requestDto.SortBy))
            {
                requestDto.SortBy = "totalSpent";
            }
            if (string.IsNullOrWhiteSpace(requestDto.SortDirection) || 
                (requestDto.SortDirection.ToLower() != "asc" && requestDto.SortDirection.ToLower() != "desc"))
            {
                requestDto.SortDirection = "desc";
            }

        }

        public static ValidationResult Validate(this CustomerOrderSummaryRequestDto requestDto)
        {
            var result = new ValidationResult();
            if (requestDto.PageNumber <= 0)
            {
                result.Errors.Add("PageNumber must be greater than 0.");
            }
            if (requestDto.PageSize <= 0)
            {
                result.Errors.Add("PageSize must be greater than 0.");
            }
            if (requestDto.PageSize > 100)
            {
                result.Errors.Add("PageSize cannot be greater than 100.");
            }
            if (requestDto.MinOrderCount < 0)
            {
                result.Errors.Add("MinOrderCount cannot be negative.");
            }
            if (requestDto.MinTotalSpent < 0)
            {
                result.Errors.Add("MinTotalSpent cannot be negative.");
            }

            return result;
        }
    }
}
