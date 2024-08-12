namespace Domain.Entities;

public class Customer
{
	public int Id { get; set; }
	public string? Name { get; set; }

	// Relationship: One customer can have multiple bank accounts
	public List<BankAccount>? BankAccounts { get; set; }
}
