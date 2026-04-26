using PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Accounts
{
    public class AccountService : IAccountService
    {
        public Task<AccountBalanceDto>? GetBalanceAsync(int accountId)
        {
            var result = new AccountBalanceDto();
            result.AccountId = accountId;
            result.AvailableBalance = 2300.25m;
            result.CurrentBalance = 2450.75m;
            result.LastUpdated = DateTime.UtcNow;

            return Task.FromResult(result);
        }

        public Task<AccountHeaderDto>? GetHeaderAsync(int accountId)
        {
            var result = new AccountHeaderDto();
            result.AccountId = accountId;
            result.MaskedAccountNumber = "***1234";
            result.AccountName = "Everyday checking";
            result.AccountType = "Checking";

            return Task.FromResult(result);
        }

        public Task<List<AccountQuickActionDto>> GetQuickActionsAsync(int accountId)
        {
            var result = new List<AccountQuickActionDto>();
            
            var quickAction1 = new AccountQuickActionDto();
            quickAction1.Label = "Transfer";
            quickAction1.ActionType = "Transfer";
            quickAction1.IsEnabled = true;
            result.Add(quickAction1);

            var quickAction2 = new AccountQuickActionDto();
            quickAction2.Label = "Pay Bill";
            quickAction2.ActionType = "pay-bill";
            quickAction2.IsEnabled = true;
            result.Add(quickAction2);

            var quickAction3 = new AccountQuickActionDto();
            quickAction3.Label = "Lock Card";
            quickAction3.ActionType = "lock-card";
            quickAction3.IsEnabled = true;
            result.Add(quickAction3);

            return Task.FromResult(result);
        }

        public Task<RecentTransactionResponseDto> GetRecentTransactionsAsync(int accountId, int pageSize, int pageNumber)
        {
            var transactions = new List<RecentTransactionDto>();
            var transactionA = new RecentTransactionDto();
            transactionA.TransactionId = 101;
            transactionA.TransactionDate = DateTime.UtcNow.AddDays(-1);
            transactionA.Description = "Grocery Store";
            transactionA.Amount = 54.25m;
            transactionA.TransactionType = "Debit";
            transactions.Add(transactionA);

            var transactionB = new RecentTransactionDto();
            transactionB.TransactionId = 102;
            transactionB.TransactionDate = DateTime.UtcNow.AddDays(-3);
            transactionB.Description = "Electric Bill";
            transactionB.Amount = -132.80m;
            transactionB.TransactionType = "Debit";
            transactions.Add(transactionB);

            var transactionC = new RecentTransactionDto();
            transactionC.TransactionId = 103;
            transactionC.TransactionDate = DateTime.UtcNow.AddDays(-5);
            transactionC.Description = "Paycheck Deposit";
            transactionC.Amount = 2500m;
            transactionC.TransactionType = "Credit";
            transactions.Add(transactionC);

            var result = new RecentTransactionResponseDto();
            result.AccountId = accountId;
            result.Transactions = transactions;
            result.PageSize = pageSize;
            result.PageNumber = pageNumber;
            result.TotalCount = transactions.Count;

            return Task.FromResult(result);
        }
    }
}
