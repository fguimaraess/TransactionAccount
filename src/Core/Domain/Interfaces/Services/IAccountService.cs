

using Domain.Entities.DTO;

namespace Domain.Interfaces;

public interface IAccountService
{
    Task<AccountBalanceDto?> GetBalanceAsync(int accountId);
    Task<StatementAccountDto?> GetStatementAsync(int accountId, DateTime startDate, DateTime endDate);
}
