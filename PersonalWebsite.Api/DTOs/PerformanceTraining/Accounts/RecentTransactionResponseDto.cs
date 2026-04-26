namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class RecentTransactionResponseDto
    {
        public int AccountId { get; set; }

        public List<RecentTransactionDto> Transactions { get; set; } = new();

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
