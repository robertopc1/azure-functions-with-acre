using Azure;
using Redis.OM;
using Redis.API.Application.Queries;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Redis.API.Application.Services;

public class RedisService : IRedisService
{
    private readonly IRedisConnectionProvider _provider;
    private readonly IRedisCollection<RedisProduct> _productCollection;

    public RedisService(IRedisConnectionProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _productCollection = _provider.RedisCollection<RedisProduct>();
    }

    public async Task<RedisProduct> GetProductByIdAsync(int id)
    {
        return await _productCollection.FirstOrDefaultAsync(x => x.Id == id);
    }
}