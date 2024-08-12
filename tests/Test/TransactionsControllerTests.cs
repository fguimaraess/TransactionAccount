using Domain.Commands;
using Domain.Entities.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionAccountAPI.Controllers;

namespace Test;

public class TransactionsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<TransactionsController>> _mockLogger;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<TransactionsController>>();
        _controller = new TransactionsController(_mockMediator.Object, _mockLogger.Object);

        // Criando um mock de HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = $"/transactions";
        // Associando o HttpContext ao ControllerContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task CreateTransaction_ReturnsBadRequest_WhenCommandIsNull()
    {
        // Arrange
        CreateTransactionCommand? command = null;

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Invalid transaction data", problemDetails.Title);
        Assert.Equal("The transaction data provided is null or invalid.", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsBadRequest_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 0,  // Invalid amount
            Description = "Test transaction"
        };

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Invalid transaction data", problemDetails.Title);
        Assert.Equal("The transaction data provided is null or invalid.", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsBadRequest_WhenSourceAccountIdIsZero()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 0,  // Invalid account id
            Amount = 100,
            Description = "Test transaction"
        };

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Invalid transaction data", problemDetails.Title);
        Assert.Equal("The transaction data provided is null or invalid.", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsBadRequest_WhenTransactionFails()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test transaction"
        };

        var resultMock = new TransactionResult(false, "Insufficient funds.");

        _mockMediator.Setup(m => m.Send(command, default))
            .ReturnsAsync(resultMock);

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Transaction failed", problemDetails.Title);
        Assert.Equal("Insufficient funds.", problemDetails.Detail);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsOk_WhenTransactionDebitIsSuccessful()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test transaction",
            Type = Domain.Entities.Enum.TransactionType.Debit
        };

        var resultMock = new TransactionResult(true, "Transaction completed successfully.");

        _mockMediator.Setup(m => m.Send(command, default))
            .ReturnsAsync(resultMock);

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transactionResult = Assert.IsType<TransactionResult>(okResult.Value);
        Assert.True(transactionResult.IsSuccess);
        Assert.Equal("Transaction completed successfully.", transactionResult.Message);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsOk_WhenTransactionCreditIsSuccessful()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test transaction",
            Type = Domain.Entities.Enum.TransactionType.Credit
        };

        var resultMock = new TransactionResult(true, "Transaction completed successfully.");

        _mockMediator.Setup(m => m.Send(command, default))
            .ReturnsAsync(resultMock);

        // Act
        var result = await _controller.CreateTransaction(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transactionResult = Assert.IsType<TransactionResult>(okResult.Value);
        Assert.True(transactionResult.IsSuccess);
        Assert.Equal("Transaction completed successfully.", transactionResult.Message);
    }
}
