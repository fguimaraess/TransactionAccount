using API.Controllers;
using Azure.Core;
using Domain.Entities.DTO;
using Domain.Interfaces;
using Domain.Interfaces.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<ILogger<AccountsController>> _mockLogger;
    private readonly Mock<ICacheService> _mockCache;
    private readonly AccountsController _controller;

    public AccountControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _mockCache = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<AccountsController>>();
        _controller = new AccountsController(_mockAccountService.Object, _mockLogger.Object, _mockCache.Object);
    }

    [Fact]
    public async Task GetStatement_ReturnsBadRequest_WhenDateRangeExceeds90Days()
    {
        // Arrange
        int accountId = 1;
        DateTime startDate = new(2024, 1, 1);
        DateTime endDate = new(2024, 4, 2); // 92 days apart

        // Criando um mock de HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = $"/accounts/{accountId}/statement";
        // Associando o HttpContext ao ControllerContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.GetStatement(accountId, startDate, endDate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Date range {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd} cannot exceed 90 days.", ((ProblemDetails?)badRequestResult.Value)?.Detail);
    }

    [Fact]
    public async Task GetStatement_ReturnsOk_WhenDateRangeIsValid()
    {
        // Arrange
        int accountId = 1;
        DateTime startDate = new(2024, 1, 1);
        DateTime endDate = new(2024, 3, 31); // 90 days apart

        _mockAccountService.Setup(s => s.GetStatementAsync(accountId, startDate, endDate))
            .ReturnsAsync(new StatementAccountDto());

        // Act
        var result = await _controller.GetStatement(accountId, startDate, endDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<StatementAccountDto>(okResult.Value);
    }

    [Fact]
    public async Task GetBalance_ReturnsValueFromCache_WhenValueIsInCache()
    {
        // Arrange
        int accountId = 1;
        AccountBalanceDto accountBalanceDto = new() { Balance = 1000m };

        // Configurando o mock do cache para retornar o valor esperado
        _mockCache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto>>>()))
            .ReturnsAsync(accountBalanceDto);

        // Act
        var result = await _controller.GetBalance(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accountBalanceDto, okResult.Value);

        // Verificando se o cache foi consultado
        _mockCache.Verify(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto>>>()), Times.Once);

        // Verificando se o serviço NÃO foi chamado, já que o valor está no cache
        _mockAccountService.Verify(s => s.GetBalanceAsync(accountId), Times.Never);
    }

    [Fact]
    public async Task GetBalance_ReturnsValueFromDatabase_WhenValueIsNotInCache()
    {
        // Arrange
        int accountId = 1;
        AccountBalanceDto accountBalanceDto = new() { Balance = 1000m };

        // Configurando o mock do cache para retornar null, simulando que o valor não está no cache
        _mockCache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto?>>>()))
            .Returns((string key, Func<ICacheEntry, Task<AccountBalanceDto?>> factory) => factory(new Mock<ICacheEntry>().Object));

        // Configurando o mock do serviço para retornar o valor da base de dados
        _mockAccountService.Setup(s => s.GetBalanceAsync(accountId))
            .ReturnsAsync(accountBalanceDto);

        // Act
        var result = await _controller.GetBalance(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accountBalanceDto, okResult.Value);

        // Verificando se o cache foi consultado
        _mockCache.Verify(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto>>>()), Times.Once);

        // Verificando se o serviço foi chamado, já que o valor não estava no cache
        _mockAccountService.Verify(s => s.GetBalanceAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetBalance_ReturnsOk_WhenAccountExists()
    {
        // Arrange
        int accountId = 1;
        AccountBalanceDto accountBalanceDto = new() { Balance = 1000m };

        // Configurando o mock do cache para retornar o valor esperado
        _mockCache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto>>>()))
            .ReturnsAsync(accountBalanceDto);

        // Act
        var result = await _controller.GetBalance(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accountBalanceDto, okResult.Value);

        // Verificando se o cache foi consultado
        _mockCache.Verify(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto>>>()), Times.Once);

        // Verificando se o serviço foi chamado (neste caso, não deveria ser chamado, já que o cache contém o valor)
        _mockAccountService.Verify(s => s.GetBalanceAsync(accountId), Times.Never);
    }


    [Fact]
    public async Task GetBalance_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        int accountId = 1;
        AccountBalanceDto? accountBalanceDto = null;

        // Mock da resposta do serviço de conta
        _mockAccountService.Setup(s => s.GetBalanceAsync(accountId))
            .ReturnsAsync(accountBalanceDto);

        // Mock do cache que simula a ausência do valor no cache e chama o serviço
        _mockCache.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto?>>>()))
            .Returns((string key, Func<ICacheEntry, Task<AccountBalanceDto?>> factory) => factory(new Mock<ICacheEntry>().Object));
       
        // Criando um mock de HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = $"/accounts/{accountId}/balance";
        // Associando o HttpContext ao ControllerContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.GetBalance(accountId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);

        // Verifica se o cache foi consultado e se o serviço foi chamado
        _mockCache.Verify(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<ICacheEntry, Task<AccountBalanceDto?>>>()), Times.Once);
        _mockAccountService.Verify(s => s.GetBalanceAsync(accountId), Times.Once);
    }
}
