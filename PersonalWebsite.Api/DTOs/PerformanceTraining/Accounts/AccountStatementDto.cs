namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class AccountStatementDto
    {
        public int StatementId { get; set; }

        public int AccountId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string StatementName { get; set; } = string.Empty;

        public DateTime StatementDate { get; set; }

        public string DownloadUrl { get; set; } = string.Empty;
    }
}
