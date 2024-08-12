using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.Services;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName} with data: {@Request}", typeof(TRequest).Name, JsonConvert.SerializeObject(request));
        var response = await next();
        _logger.LogInformation("Handled {RequestName} with data: {@Request}. Response: {@Response}", typeof(TRequest).Name, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));

        return response;
    }
}