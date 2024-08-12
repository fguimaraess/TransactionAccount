using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Interfaces.Data;
using Domain.Services;
using Moq;

namespace Test;

public class AccountServiceTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
        _service = new AccountService(_mockTransactionRepository.Object, _mockBankAccountRepository.Object);
    }

    [Fact]
    public async Task GetBalance_ReturnsCorrectBalance_WhenAccountExists()
    {
        int accountId = 1;
        AccountBalanceDto expectedBalance = new AccountBalanceDto { Balance = 1 };

        _mockBankAccountRepository.Setup(r => r.GetBalanceByIdAsync(accountId))
            .ReturnsAsync(new AccountBalanceDto { Balance = 1 });

        var balance = await _service.GetBalanceAsync(accountId);

        Assert.Equal(expectedBalance.Balance, balance?.Balance);
    }

    [Fact]
    public async Task GetStatement_ReturnsTransactions_WhenAccountAndDateRangeIsValid()
    {
        int accountId = 1;
        DateTime startDate = new DateTime(2024, 1, 1);
        DateTime endDate = new DateTime(2024, 3, 31);

        _mockTransactionRepository.Setup(r => r.GetByAccountIdAndDateRangeAsync(accountId, startDate, endDate))
            .ReturnsAsync(new StatementAccountDto { TransactionList = [new Transaction { SourceAccountId = 1 }] });

        var statement = await _service.GetStatementAsync(accountId, startDate, endDate);
        List<Transaction> resultList = statement?.TransactionList ?? [];

        Assert.NotNull(statement);
        Assert.NotEmpty(resultList);
    }
}
