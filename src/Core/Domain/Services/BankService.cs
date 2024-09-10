using Domain.Entities.DTO;
using Domain.Interfaces;
using Domain.Interfaces.RestService;

namespace Domain.Services;

public class BankService : IBankService
{
    private readonly IBankApi _bankApi;

    public BankService(IBankApi bankApi)
    {
        _bankApi = bankApi;
    }

    public BankDTO GetBankByCode(int code)
    {
        return _bankApi.GetBankByCode(code);
    }
}
