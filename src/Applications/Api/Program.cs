using Domain.Handlers;
using Domain.Interfaces;
using Domain.Interfaces.Data;
using Domain.Interfaces.RestService;
using Domain.Interfaces.Services;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.RestService;
using MediatR;
using Microsoft.Data.SqlClient;
using Polly;
using ServiceDefaults;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurações de Serviços

// Configuração da conexão com o banco de dados usando Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("MY_DB")));

builder.Services.AddMemoryCache();

// Registra os repositórios
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IBankApi, BankApi>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();

// Registra os processadores de transações
builder.Services.AddTransient<ITransactionProcessor, DebitTransactionProcessor>();
builder.Services.AddTransient<ITransactionProcessor, CreditTransactionProcessor>();

// Registra o MediatR e os handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransactionHandler).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Adiciona os controllers e outras configurações da API
builder.Services.AddHttpClient();
builder.Services.AddControllers();

// Adiciona o Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServiceDefaults();

builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new Polly.Retry.RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        Delay = TimeSpan.FromSeconds(2),
        MaxRetryAttempts = 2,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
    }).AddTimeout(TimeSpan.FromSeconds(30));
});

var app = builder.Build();

// Configuração do Pipeline de Middleware

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
