using PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Accounts
{
    public interface IAccountService
    {
        Task<AccountHeaderDto>? GetHeaderAsync(int accountId);
        Task<AccountBalanceDto>? GetBalanceAsync(int accountId);
        Task<List<AccountQuickActionDto>> GetQuickActionsAsync(int accountId);
        Task<RecentTransactionResponseDto> GetRecentTransactionsAsync(int accountId, int pageSize, int pageNumber);
    }
}
