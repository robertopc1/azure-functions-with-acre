using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Shared;


namespace DataAccess.DbAccess;

public class SQLDataAccess : ISQLDataAccess
{
    private readonly IConfiguration _config;

    public SQLDataAccess(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task SaveData<T>(
        string storedProcedure,
        T parameters,
        string connectionId = Common.SQLConnectionString)
    {
        using IDbConnection connection = new SqlConnection(_config[connectionId]);

        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }
}