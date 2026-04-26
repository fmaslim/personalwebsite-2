namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class SpendingSummaryDto
    {
        public int AccountId { get; set; }

        public decimal TotalSpentThisMonth { get; set; }

        public decimal TotalIncomeThisMonth { get; set; }

        public decimal NetCashFlow { get; set; }

        public string TopSpendingCategory { get; set; } = string.Empty;
    }
}
