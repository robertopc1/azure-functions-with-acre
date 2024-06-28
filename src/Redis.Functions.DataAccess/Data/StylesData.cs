using Redis.Functions.DataAccess.DbAccess;
using Redis.Functions.DataAccess.Models;
using Redis.Functions.Shared;

namespace Redis.Functions.DataAccess.Data;

public class StylesData : IStylesData
{
    private readonly ISQLDataAccess _db;

    public StylesData(ISQLDataAccess db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public Task UpdateStyle(Styles style) =>
        _db.SaveData(Common.StoreProcedureUpdate, style);
}