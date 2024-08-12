using Domain.Entities.Enum;

namespace Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int SourceAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public TransactionType Type { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime InsertDate { get; set; }
}


