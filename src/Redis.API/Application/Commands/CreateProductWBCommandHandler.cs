using MediatR;
using Microsoft.Extensions.Options;
using Redis.API.Application.Queries;
using Redis.API.Infrastructure.Helpers;
using Redis.Domain.AggregatesModel.ProductAggregate;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Redis.API.Application.Commands;

public class CreateProductWBCommandHandler
    : IRequestHandler<CreateProductCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateProductWBCommandHandler> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IOptions<Settings> _settings;
    private readonly IRedisConnectionProvider _provider;
    private readonly IRedisCollection<RedisProduct> _productCollection;

    public CreateProductWBCommandHandler(IMediator mediator, 
        ILogger<CreateProductWBCommandHandler> logger, 
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<Settings> settings,
        IRedisConnectionProvider provider)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _productCollection = _provider.RedisCollection<RedisProduct>();

    }

    public async Task<bool> Handle(CreateProductCommand message, CancellationToken cancellationToken)
    {
        //_logger.LogInformation("----- Creating Product - Product: {@Product}", product);
        
        try
        {
            // Handle Id Generation
            var maxId = await _mediator.Send(new GetMaxIdQuery(), cancellationToken);
            var newId = maxId + 1;
            message.SetId(newId);
            
            var db = _connectionMultiplexer.GetDatabase();

            var product = new RedisProduct()
            {
                Id = message.Id,
                Gender = message.Gender,
                MasterCategory = message.MasterCategory,
                SubCategory = message.SubCategory,
                ArticleType = message.ArticleType,
                BaseColour = message.BaseColour,
                Season = message.Season,
                Year = message.Year,
                Usage = message.Usage,
                ProductDisplayName = message.ProductDisplayName
            };
            
            await _productCollection
                .InsertAsync(product);
            
            // Insert to stream for Azure Function to Pick Up as Write-Behind
            
            var entries = RedisHelper.ToNameValueEntries(message);
            var result = await db.StreamAddAsync(_settings.Value.WriteBehindStreamName, entries);
        }
        catch (Exception ex)
        {
            // Handle failure if something goes wrong. If Key was added but stream the data. 
        }

        
        return true;
    }
}