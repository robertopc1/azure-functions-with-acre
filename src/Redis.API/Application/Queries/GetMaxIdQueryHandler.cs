using AutoMapper;
using MediatR;
using Redis.API.Application.Queries.DTOs;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Redis.API.Application.Queries;

public class GetMaxIdQueryHandler : IRequestHandler<GetMaxIdQuery, int>
{
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRedisConnectionProvider _provider;
    private readonly IRedisCollection<RedisProduct> _productCollection;
    
    public GetMaxIdQueryHandler(IMapper mapper, 
        ILogger<GetProductByIdQueryHandler> logger,
        IRedisConnectionProvider provider)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _productCollection = _provider.RedisCollection<RedisProduct>();
    }
    
    public async Task<int> Handle(GetMaxIdQuery message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Getting Max Id: {@Id}", message);

        var id =  _productCollection.Max(x => x.Id); 

        return id;
    }
}