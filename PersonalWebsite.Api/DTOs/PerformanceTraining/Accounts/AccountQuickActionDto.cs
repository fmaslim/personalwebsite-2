namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts
{
    public class AccountQuickActionDto
    {
        public string Label { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }
}
