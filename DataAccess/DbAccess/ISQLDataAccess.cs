using Shared;

namespace DataAccess.DbAccess;

public interface ISQLDataAccess
{
    Task SaveData<T>(string storedProcedure, T parameters, string connectionId = Common.SQLConnectionString);
}