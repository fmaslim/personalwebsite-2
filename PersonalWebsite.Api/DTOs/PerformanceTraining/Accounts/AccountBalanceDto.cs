namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class AccountBalanceDto
    {
        public int AccountId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
