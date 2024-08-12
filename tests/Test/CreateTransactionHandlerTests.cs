using Domain.Commands;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Handlers;
using Domain.Interfaces.Data;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Test;

public class CreateTransactionHandlerTests
{
    private readonly Mock<IBankAccountRepository> _mockAccountRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly CreateTransactionHandler _handler;

    public CreateTransactionHandlerTests()
    {
        _mockAccountRepository = new Mock<IBankAccountRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockCache = new Mock<IMemoryCache>();
        _handler = new CreateTransactionHandler(_mockTransactionRepository.Object, _mockAccountRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task Handle_DebitTransaction_ReturnsSuccess_WhenBalanceIsSufficient()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test Debit",
            Type = TransactionType.Debit
        };

        _mockAccountRepository.Setup(r => r.GetByIdAsync(command.SourceAccountId))
            .ReturnsAsync(new BankAccount { Id = 1, Balance = 200 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Transaction completed successfully.", result.Message);
    }

    [Fact]
    public async Task Handle_DebitTransaction_ReturnsFailure_WhenBalanceIsInsufficient()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 300,
            Description = "Test Debit",
            Type = TransactionType.Debit
        };

        _mockAccountRepository.Setup(r => r.GetByIdAsync(command.SourceAccountId))
            .ReturnsAsync(new BankAccount { Id = 1, Balance = 200 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Transaction failed: Insufficient funds.", result.Message);
    }

    [Fact]
    public async Task Handle_CreditTransaction_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test Credit",
            Type = TransactionType.Credit
        };

        _mockAccountRepository.Setup(r => r.GetByIdAsync(command.SourceAccountId))
            .ReturnsAsync(new BankAccount { Id = 1, Balance = 200 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Transaction completed successfully.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenSourceAccountIdIsZero()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 0, // Invalid account ID
            Amount = 100,
            Description = "Test Debit",
            Type = TransactionType.Debit
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid source account.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenSourceAccountNotFound()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test Debit",
            Type = TransactionType.Debit
        };

        _mockAccountRepository.Setup(r => r.GetByIdAsync(command.SourceAccountId))
            .ReturnsAsync((BankAccount?)null); // Account not found

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid source account.", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreateTransactionCommand
        {
            SourceAccountId = 1,
            Amount = 100,
            Description = "Test Debit",
            Type = TransactionType.Debit
        };

        // Configura o mock para lançar uma exceção quando GetByIdAsync for chamado
        _mockAccountRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Transaction failed: An unexpected error occurred", result.Message);
    }

}
