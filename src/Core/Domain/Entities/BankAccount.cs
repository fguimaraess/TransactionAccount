namespace Domain.Entities;

public class BankAccount
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Balance { get; set; }
}
