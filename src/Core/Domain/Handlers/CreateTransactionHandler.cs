using Domain.Commands;
using Domain.Entities;
using Domain.Entities.Response;
using Domain.Factory;
using Domain.Interfaces.Data;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Handlers;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionResult>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IMemoryCache _cache;

    public CreateTransactionHandler(ITransactionRepository transactionRepository, IBankAccountRepository bankAccountRepository, IMemoryCache cache)
    {
        _transactionRepository = transactionRepository;
        _bankAccountRepository = bankAccountRepository;
        _cache = cache;
    }

    public async Task<TransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        using var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var sourceAccount = await _bankAccountRepository.GetByIdAsync(request.SourceAccountId);

            if (sourceAccount == null)
            {
                return new TransactionResult(false, "Invalid source account.");
            }

            // Obtém o processador correto com base no tipo de transação
            var processor = TransactionProcessorFactory.GetProcessor(request.Type);
            processor.Process(sourceAccount, request.Amount);

            // Cria a transação
            var transaction = new Transaction
            {
                SourceAccountId = request.SourceAccountId,
                Amount = request.Amount,
                Description = request.Description,
                TransactionDate = DateTime.UtcNow.Date,
                InsertDate = DateTime.UtcNow,
                Type = request.Type
            };

            await _transactionRepository.AddAsync(transaction);
            await _bankAccountRepository.UpdateAsync(sourceAccount);

            _cache.Remove($"Balance_{sourceAccount.Id}"); //Removo para garantir que ao chamar a API de Get Balance, o usuário sempre terá o saldo atualizado.
            
            transactionScope.Complete();
            return new TransactionResult(true, "Transaction completed successfully.");
        }
        catch (Exception ex)
        {
            return new TransactionResult(false, $"Transaction failed: {ex.Message}");
        }
    }
}
