using MediatR;
using Redis.Domain.AggregatesModel.ProductAggregate;

namespace Redis.API.Application.Commands;

public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IMediator mediator, 
        ILogger<CreateProductCommandHandler> logger,
        IProductRepository productRepository)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<bool> Handle(CreateProductCommand message, CancellationToken cancellationToken)
    {
        var product = new Product(
            message.Id,
            message.Gender,
            message.MasterCategory,
            message.SubCategory,
            message.ArticleType,
            message.BaseColour,
            message.Season,
            message.Year,
            message.Usage,
            message.ProductDisplayName);
        
        _logger.LogInformation("----- Creating Product - Product: {@Product}", product);

        _productRepository.Add(product);

        return await _productRepository.UnitOfWork
            .SaveEntitiesAsync(cancellationToken);   
    }

}