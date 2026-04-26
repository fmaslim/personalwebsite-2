namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class AccountHeaderDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string MaskedAccountNumber { get; set; } = string.Empty;
    }
}
