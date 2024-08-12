using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface ITransactionProcessor
{
    void Process(BankAccount account, decimal amount);

}
