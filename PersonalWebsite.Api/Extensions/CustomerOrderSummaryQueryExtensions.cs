using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;

namespace PersonalWebsite.Api.Extensions
{
    public static class CustomerOrderSummaryQueryExtensions
    {
        public static IQueryable<CustomerOrderSummaryResultDto> ApplySorting(this IQueryable<CustomerOrderSummaryResultDto> query, CustomerOrderSummaryRequestDto requestDto)
        {
            query = requestDto.SortBy?.ToLower() switch
            {
                "ordercount" => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.OrderCount)
                    : query.OrderByDescending(x => x.OrderCount),
                "lastorderdate" => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.LastOrderDate)
                    : query.OrderByDescending(x => x.LastOrderDate),
                _ => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.TotalSpent)
                    : query.OrderByDescending(x => x.TotalSpent)
            };

            return query;
        }

        public static IQueryable<CustomerOrderSummaryResultDto> ApplyPaging(this IQueryable<CustomerOrderSummaryResultDto> query, CustomerOrderSummaryRequestDto requestDto)
        {
            return query
                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                .Take(requestDto.PageSize);
        }
    }
}
