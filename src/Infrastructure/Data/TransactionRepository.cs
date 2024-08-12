using Dapper;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Interfaces.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Data;

public class TransactionRepository : ITransactionRepository
{
    private readonly IDbConnection _dbConnection;

    public TransactionRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task AddAsync(Transaction transaction)
    {
        var sql = "INSERT INTO ACCOUNT_TRANSACTION (SourceAccountId, Amount, Description, TransactionDate, Type, InsertDate) " +
                  "VALUES (@SourceAccountId, @Amount, @Description, @TransactionDate, @Type, @InsertDate)";
        await _dbConnection.ExecuteAsync(sql, transaction);
    }

    public async Task<StatementAccountDto?> GetByAccountIdAndDateRangeAsync(int accountId, DateTime startDate, DateTime endDate)
    {
        var sql = @"    SELECT	A.AccountNumber, A.Balance, C.Id as CustomerId, C.Name
                        FROM	ACCOUNT A WITH(NOLOCK)
                        JOIN	CUSTOMER C WITH(NOLOCK) on A.CustomerId = C.Id
                        WHERE	A.Id = @AccountId

                        SELECT  Id, SourceAccountId, Amount, Description, TransactionDate, Type, InsertDate
                        FROM    ACCOUNT_TRANSACTION WITH(NOLOCK)
                        WHERE   SourceAccountId = @AccountId
                        AND     TransactionDate BETWEEN @StartDate AND @EndDate;";

        StatementAccountDto? result = null;
        using var multi = await _dbConnection.QueryMultipleAsync(sql, new { AccountId = accountId, StartDate = startDate, EndDate = endDate });

        var accountDetail = multi.Read<AccountDetailDto>();
        if (accountDetail.Any())
        {
            result = new();
            result.AccountDetail =accountDetail.Single();
            result.TransactionList = multi.Read<Transaction>()?.AsList();
        }

        return result;
    }

}
