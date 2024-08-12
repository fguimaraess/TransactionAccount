using Domain.Entities;
using Domain.Interfaces.Services;

namespace Domain.Services;
public class CreditTransactionProcessor : ITransactionProcessor
{
    public void Process(BankAccount account, decimal amount)
    {
        account.Balance += amount;
    }
}
