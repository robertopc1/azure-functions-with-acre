using AutoMapper;
using MediatR;
using Redis.API.Application.Queries.DTOs;
using Redis.Domain.AggregatesModel.ProductAggregate;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Redis.API.Application.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, RedisProductDTO>
{
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRedisConnectionProvider _provider;
    private readonly IRedisCollection<RedisProduct> _productCollection;
    
    public GetProductByIdQueryHandler(IMapper mapper, 
        ILogger<GetProductByIdQueryHandler> logger,
        IRedisConnectionProvider provider)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _productCollection = _provider.RedisCollection<RedisProduct>();
    }
    
    public async Task<RedisProductDTO> Handle(GetProductByIdQuery message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Getting Product - Product Id: {@Product}", message);

        var product = await _productCollection.FirstOrDefaultAsync(x => x.Id == message.Id); 

        var productDto = _mapper.Map<RedisProductDTO>(product);

        return productDto;
    }
}