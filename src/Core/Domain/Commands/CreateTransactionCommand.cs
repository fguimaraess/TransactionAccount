using MediatR;
using Domain.Entities.Enum;
using Domain.Entities.Response;

namespace Domain.Commands;

public class CreateTransactionCommand : IRequest<TransactionResult>
{
    public int SourceAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public TransactionType Type { get; set; }
}