using AutoMapper;
using MediatR;
using Redis.API.Application.Queries.DTOs;
using Redis.Domain.AggregatesModel.ProductAggregate;

namespace Redis.API.Application.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, RedisProductDTO>
{
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    private readonly IRedisService _redisService;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IMapper mapper, 
        ILogger<GetProductByIdQueryHandler> logger,
        IRedisService redisService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
    }
    
    public async Task<RedisProductDTO> Handle(GetProductByIdQuery message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Getting Product - Product Id: {@Product}", message);

        var product = await _redisService
           .GetProductByIdAsync(message.Id);

        var productDto = _mapper.Map<RedisProductDTO>(product);

        return productDto;
    }
}