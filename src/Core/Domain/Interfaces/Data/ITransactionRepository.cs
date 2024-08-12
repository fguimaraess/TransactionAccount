using Domain.Entities;
using Domain.Entities.DTO;

namespace Domain.Interfaces.Data;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<StatementAccountDto?> GetByAccountIdAndDateRangeAsync(int accountId, DateTime startDate, DateTime endDate);
    
}
