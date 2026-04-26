using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Accounts
{
    public interface IAccountService
    {
        Task<AccountHeaderDto>? GetHeaderAsync(int accountId);
        Task<AccountBalanceDto>? GetBalanceAsync(int accountId);
        Task<List<AccountQuickActionDto>> GetQuickActionsAsync(int accountId);
        Task<PagedResponse<RecentTransactionDto>> GetRecentTransactionsAsync(int accountId, RecentTransactionRequestDto requestDto);
        Task<SpendingSummaryDto> GetSpendingSummaryAsync(int accountId);
        Task<List<LinkedAccountDto>> GetLinkedAccountsAsync(int accountId);
        Task<List<AccountStatementDto>> GetAccountStatementsAsync(int accountId, int year);
    }
}
