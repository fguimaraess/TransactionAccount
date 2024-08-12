using Domain.Interfaces.Data;
using Domain.Entities.DTO;
using Domain.Interfaces;

namespace Domain.Services;

public class AccountService : IAccountService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBankAccountRepository _bankAccountRepository;

    public AccountService(ITransactionRepository transactionRepository, IBankAccountRepository bankAccountRepository)
    {
        _transactionRepository = transactionRepository;
        _bankAccountRepository = bankAccountRepository;
    }

    public async Task<AccountBalanceDto?> GetBalanceAsync(int accountId)
    {
        var account = await _bankAccountRepository.GetBalanceByIdAsync(accountId);
        return account;
    }

    public async Task<StatementAccountDto?> GetStatementAsync(int accountId, DateTime startDate, DateTime endDate)
    {
        var transactions = await _transactionRepository.GetByAccountIdAndDateRangeAsync(accountId, startDate, endDate);
        return transactions;
    }
}
