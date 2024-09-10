using Domain.Entities.DTO;

namespace Domain.Interfaces;

public interface IBankService
{
    public BankDTO GetBankByCode(int code);
}
