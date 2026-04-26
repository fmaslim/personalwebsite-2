namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class RecentTransactionDto
    {
        public int TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string TransactionType { get; set; } = string.Empty;
    }
}
