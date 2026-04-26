using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Accounts
{
    public class AccountService : IAccountService
    {
        public Task<List<AccountStatementDto>> GetAccountStatementsAsync(int accountId, int year)
        {
            var accountStatements = new List<AccountStatementDto>();

            var statementA = new AccountStatementDto();
            statementA.StatementId = 301;
            statementA.AccountId = accountId;
            statementA.Year = year;
            statementA.Month = 1;
            statementA.StatementName = "January Statement";
            statementA.StatementDate = new DateTime(year, 1, 31);
            statementA.DownloadUrl = $"/api/accounts/{accountId}/statements/{301}/download";
            accountStatements.Add(statementA);

            var statementB = new AccountStatementDto();
            statementB.StatementId = 302;
            statementB.AccountId = accountId;
            statementB.Year = year;
            statementB.Month = 2;
            statementB.StatementName = "February Statement";
            statementB.StatementDate = new DateTime(year, 2, 28);
            statementB.DownloadUrl = $"/api/accounts/{accountId}/statements/{302}/download";
            accountStatements.Add(statementB);

            var statementC = new AccountStatementDto();
            statementC.StatementId = 303;
            statementC.AccountId = accountId;
            statementC.Year = year;
            statementC.Month = 3;
            statementC.StatementName = "March Statement";
            statementC.StatementDate = new DateTime(year, 3, 31);
            statementC.DownloadUrl = $"/api/accounts/{accountId}/statements/{303}/download";
            accountStatements.Add(statementC);

            return Task.FromResult(accountStatements);
        }

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

        public Task<List<LinkedAccountDto>> GetLinkedAccountsAsync(int accountId)
        {
            var linkedAccounts = new List<LinkedAccountDto>();

            var accountA = new LinkedAccountDto();
            accountA.LinkedAccountId = 201;
            accountA.InstitutionName = "Chase";
            accountA.AccountName = "Everyday Checking";
            accountA.AccountType = "Checking";
            accountA.MaskedAccountNumber = "****1234";
            accountA.Balance = 2450.75m;
            accountA.IsActive = true;
            linkedAccounts.Add(accountA);

            var accountB = new LinkedAccountDto();
            accountB.LinkedAccountId = 202;
            accountB.InstitutionName = "Capital One";
            accountB.AccountName = "High Yield Savings";
            accountB.AccountType = "Savings";
            accountB.MaskedAccountNumber = "****9876";
            accountB.Balance = 8750.00m;
            accountB.IsActive = true;
            linkedAccounts.Add(accountB);

            var accountC = new LinkedAccountDto();
            accountC.LinkedAccountId = 203;
            accountC.InstitutionName = "Fidelity";
            accountC.AccountName = "Brokerage Account";
            accountC.AccountType = "Brokerage";
            accountC.MaskedAccountNumber = "****4567";
            accountC.Balance = 15200.40m;
            accountC.IsActive = true;
            linkedAccounts.Add(accountC);

            return Task.FromResult(linkedAccounts);
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

        public Task<PagedResponse<RecentTransactionDto>> GetRecentTransactionsAsync(int accountId, RecentTransactionRequestDto requestDto)
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

            var pagedTransactions = transactions
            .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
            .Take(requestDto.PageSize)
            .ToList();

            var finalResult = new PagedResponse<RecentTransactionDto>
            {
                Data = pagedTransactions,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = transactions.Count,
                TotalPages = (int)Math.Ceiling(transactions.Count / (double)requestDto.PageSize)
            };

            return Task.FromResult(finalResult);
        }

        public Task<SpendingSummaryDto> GetSpendingSummaryAsync(int accountId)
        {
            var summary = new SpendingSummaryDto();
            summary.AccountId = accountId;
            summary.TotalSpentThisMonth = 12450.50m;
            summary.TotalIncomeThisMonth = 3200.00m;
            summary.NetCashFlow = summary.TotalIncomeThisMonth - summary.TotalSpentThisMonth;
            summary.TopSpendingCategory = "Groceries";

            return Task.FromResult(summary);
        }
    }
}
