using Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> _loggingBehavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        _loggingBehavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogMessages()
    {
        // Arrange
        var inMemoryLogger = new InMemoryLogger<LoggingBehavior<TestRequest, TestResponse>>();
        var loggingBehavior = new LoggingBehavior<TestRequest, TestResponse>(inMemoryLogger);
        var request = new TestRequest();
        var response = new TestResponse();

        _nextMock.Setup(n => n()).ReturnsAsync(response);

        // Act
        await loggingBehavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        var logs = inMemoryLogger.Logs.ToList();
        Assert.Contains(logs, log => log.Contains("Handling"));
        Assert.Contains(logs, log => log.Contains("Handled"));
    }
}

public class InMemoryLogger<T> : ILogger<T>
{
    private readonly List<string> _logs = new List<string>();

    public IEnumerable<string> Logs => _logs;

    // Implementação explícita da interface ILogger para o método BeginScope
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add(formatter(state, exception));
    }
}

public class TestRequest : IRequest<TestResponse>
{
}

public class TestResponse
{
}