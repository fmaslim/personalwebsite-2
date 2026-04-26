namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class LinkedAccountDto
    {
        public int LinkedAccountId { get; set; }

        public string InstitutionName { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public string MaskedAccountNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public bool IsActive { get; set; }
    }
}
