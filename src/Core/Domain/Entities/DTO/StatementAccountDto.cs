namespace Domain.Entities.DTO;

public class StatementAccountDto
{
    public AccountDetailDto? AccountDetail { get; set; }
    public List<Transaction>? TransactionList { get; set; }
}
