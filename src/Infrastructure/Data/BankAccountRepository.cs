

using Dapper;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Interfaces.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Data;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly IDbConnection _dbConnection;

    public BankAccountRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<BankAccount?> GetByIdAsync(int id)
    {
        var sql = @"    SELECT	A.Id, A.AccountNumber, A.Balance
                        FROM	ACCOUNT A WITH(NOLOCK)
                        WHERE	A.Id = @AccountId";
        var result = await _dbConnection.QuerySingleOrDefaultAsync<BankAccount>(sql, new { AccountId = id });

        return result;
    }

    public async Task<AccountBalanceDto?> GetBalanceByIdAsync(int id)
    {
        var sql = @"    SELECT	A.AccountNumber, A.Balance, C.Id as CustomerId, C.Name
                        FROM	ACCOUNT A WITH(NOLOCK)
                        JOIN	CUSTOMER C WITH(NOLOCK) on A.CustomerId = C.Id
                        WHERE	A.Id = @AccountId";
        var result = await _dbConnection.QuerySingleOrDefaultAsync<AccountBalanceDto?>(sql, new { AccountId = id });

        return result;
    }

    public async Task UpdateAsync(BankAccount bankAccount)
    {
        var sql = "UPDATE Account SET Balance = @Balance WHERE Id = @Id";
        await _dbConnection.ExecuteAsync(sql, bankAccount);
    }
}