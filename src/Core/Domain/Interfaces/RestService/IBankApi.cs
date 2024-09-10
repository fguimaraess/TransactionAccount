using Domain.Entities.DTO;

namespace Domain.Interfaces.RestService;

public interface IBankApi
{
    public BankDTO GetBankByCode(int code);
}
