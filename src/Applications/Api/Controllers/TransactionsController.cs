using Domain.Commands;
using Domain.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TransactionAccountAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand? command)
    {
        _logger.LogInformation("[Initialize] CreateTransaction for accountId: {0}", command?.SourceAccountId);
        if (command == null || command.Amount <= 0 || command.SourceAccountId == 0)
        {
            _logger.LogInformation("[Initialize] ERROR CreateTransaction: {0}", JsonConvert.SerializeObject(command));
            var problemDetails = ProblemDetailsHelper.CreateProblemDetails(HttpContext,
                                                                            "Invalid transaction data",
                                                                            $"The transaction data provided is null or invalid.",
                                                                            StatusCodes.Status400BadRequest);

            return BadRequest(problemDetails);
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            _logger.LogInformation("[End] CreateTransaction for accountId ERROR: {0} - {1}", command?.SourceAccountId, result.ErrorMessage);
            var problemDetails = ProblemDetailsHelper.CreateProblemDetails(HttpContext,
                                                                            "Transaction failed",
                                                                            result.Message,
                                                                            StatusCodes.Status400BadRequest);
            
            return BadRequest(problemDetails);
        }

        _logger.LogInformation("[End] CreateTransaction for accountId successfull: {0}", command?.SourceAccountId);

        return Ok(result);
    }
}
