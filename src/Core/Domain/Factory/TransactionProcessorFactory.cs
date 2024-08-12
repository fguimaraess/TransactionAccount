using Domain.Interfaces.Services;
using Domain.Entities.Enum;
using Domain.Services;

namespace Domain.Factory;

public static class TransactionProcessorFactory
{
    public static ITransactionProcessor GetProcessor(TransactionType type)
    {
        return type switch
        {
            TransactionType.Debit => new DebitTransactionProcessor(),
            TransactionType.Credit => new CreditTransactionProcessor(),
            _ => throw new NotSupportedException($"Transaction type {type} is not supported.")
        };
    }
}
