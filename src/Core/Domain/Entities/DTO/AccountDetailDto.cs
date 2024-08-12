namespace Domain.Entities.DTO;

public class AccountDetailDto
{
    public string? AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public int CustomerId { get; set; }
    public string? Name { get; set; }
}
