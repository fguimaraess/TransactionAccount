using Domain.Entities;
using Domain.Entities.DTO;

namespace Domain.Interfaces.Data;

public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(int id);
    Task<AccountBalanceDto?> GetBalanceByIdAsync(int id);
    Task UpdateAsync(BankAccount bankAccount);
}
