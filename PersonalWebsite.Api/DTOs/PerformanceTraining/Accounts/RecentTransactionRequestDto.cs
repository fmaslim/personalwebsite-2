using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class RecentTransactionRequestDto : PagingRequestDto
    {
        public string? Search { get; set; }

        public string? TransactionType { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public string? SortBy { get; set; }
        public string? SortDir { get; set; } = "desc";
    }
}
