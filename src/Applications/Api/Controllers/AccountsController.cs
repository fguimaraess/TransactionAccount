
using Domain.Helper;
using Domain.Interfaces;
using Domain.Interfaces.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICacheService _cache;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger, ICacheService cache)
    {
        _accountService = accountService;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet("{accountId}/balance")]
    public async Task<IActionResult> GetBalance(int accountId)
    {
        _logger.LogInformation("[Initialize] GetBalance for accountId: {0}", accountId);

        var balance = await _cache.GetOrCreateAsync($"Balance_{accountId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
            return await _accountService.GetBalanceAsync(accountId);
        });

        if (balance == null)
        {
            _logger.LogInformation("[End] GetBalance for accountId: {0} - NOT FOUND", accountId);
            var problemDetails = ProblemDetailsHelper.CreateProblemDetails(HttpContext, "Account not found", $"No account found with ID {accountId}.", StatusCodes.Status404NotFound);
            return NotFound(problemDetails);
        }

        _logger.LogInformation("[End] GetBalance for accountId: {0}", accountId);

        return Ok(balance);
    }

    [HttpGet("{accountId}/statement")]
    public async Task<IActionResult> GetStatement(int accountId, [FromQuery, Required] DateTime startDate, [FromQuery, Required] DateTime endDate)
    {
        // Valida se o período é superior a 90 dias
        _logger.LogInformation("[Initialize] GetStatement for accountId: {0}. Initial Date: {1}, end date: {2}",
            accountId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        if ((endDate - startDate).TotalDays > 90)
        {
            _logger.LogInformation("[End] GetStatement for accountId BAD REQUEST: {0}. Initial Date: {1}, end date: {2}",
            accountId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var problemDetails = ProblemDetailsHelper.CreateProblemDetails(HttpContext,
                                                                            "Range date not accepted",
                                                                            $"Date range {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd} cannot exceed 90 days.",
                                                                            StatusCodes.Status400BadRequest);

            return BadRequest(problemDetails);
        }

        var statement = await _accountService.GetStatementAsync(accountId, startDate, endDate);

        if (statement == null || statement?.TransactionList?.Count == 0)
        {
            _logger.LogInformation("[End] GetStatement for accountId NOT FOUND: {0}. Initial Date: {1}, end date: {2}",
            accountId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var problemDetails = ProblemDetailsHelper.CreateProblemDetails(HttpContext,
                                                                            "No transactions found.",
                                                                            $"No transactions found for accountId {accountId}.",
                                                                            StatusCodes.Status404NotFound);

            return NotFound(problemDetails);
        }

        _logger.LogInformation("[End] GetStatement for accountId successfull: {0}. Initial Date: {1}, end date: {2}",
            accountId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

        return Ok(statement);
    }
}
