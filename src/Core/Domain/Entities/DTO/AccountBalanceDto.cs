namespace Domain.Entities.DTO;

public class AccountBalanceDto
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Balance { get; set; }
}
