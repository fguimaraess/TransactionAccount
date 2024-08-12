using Domain.Entities;
using Domain.Interfaces.Services;

namespace Domain.Services;
public class DebitTransactionProcessor : ITransactionProcessor
{
    public void Process(BankAccount account, decimal amount)
    {
        if (account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        account.Balance -= amount;
    }
}
