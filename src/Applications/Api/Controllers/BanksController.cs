using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly IBankService _bankService;

    public BanksController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpGet("FetchBankByCode")]
    public IActionResult FetchBankByCode([FromQuery] int bankCode)
    {
        return Ok(_bankService.GetBankByCode(bankCode));
    }
}
