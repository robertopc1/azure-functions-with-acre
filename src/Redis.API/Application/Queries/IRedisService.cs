namespace Redis.API.Application.Queries;

public interface IRedisService
{
    Task<RedisProduct> GetProductByIdAsync(int id);
}