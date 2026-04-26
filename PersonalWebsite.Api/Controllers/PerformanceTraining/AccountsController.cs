using Microsoft.AspNetCore.Mvc;
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
            [FromQuery]int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            var transactions = await _accountService.GetRecentTransactionsAsync(accountId, pageSize, pageNumber);
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
    }
}
