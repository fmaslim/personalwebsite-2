using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Accounts;
using PersonalWebsite.Api.Services.PerformanceTraining.Accounts;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{accountId}/header")]
        public async Task<IActionResult> GetAccountHeader(int accountId)
        {
            var result = await _accountService.GetHeaderAsync(accountId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetAccountBalance(int accountId)
        {
            var result = await _accountService.GetBalanceAsync(accountId);
            if (result == null)
            {  
                return NotFound(); 
            }
            return Ok(result);
        }

        [HttpGet("{accountId}/quick-actions")]
        public async Task<IActionResult> GetQuickActions(int accountId)
        {
            var result = await _accountService.GetQuickActionsAsync(accountId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{accountId}/recent-transactions")]
        public async Task<IActionResult> GetRecentTransactions(int accountId, 
            [FromQuery] RecentTransactionRequestDto requestDto)
        {
            var transactions = await _accountService.GetRecentTransactionsAsync(accountId, requestDto);
            return Ok(transactions);
        }

        [HttpGet("{accountId}/spending-summary")]
        public async Task<IActionResult> GetSpendingSummary(int accountId)
        {
            var summary = await _accountService.GetSpendingSummaryAsync(accountId);
            if (summary == null)
            {
                return NotFound();
            }
            return Ok(summary);
        }

        [HttpGet("{accountId}/linked-accounts")]
        public async Task<IActionResult> GetLinkedAccounts(int accountId)
        {
            var linkedAccounts = await _accountService.GetLinkedAccountsAsync(accountId);
            return Ok(linkedAccounts);
        }

        [HttpGet("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatements(int accountId, [FromQuery] int year = 2026)
        {
            var statements = await _accountService.GetAccountStatementsAsync(accountId, year);
            if (statements == null)
            {
                return NotFound();
            }
            return Ok(statements);
        }
    }
}
