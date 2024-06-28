using Redis.Functions.DataAccess.Models;

namespace Redis.Functions.DataAccess.Data;

public interface IStylesData
{
    Task UpdateStyle(Styles style);
}