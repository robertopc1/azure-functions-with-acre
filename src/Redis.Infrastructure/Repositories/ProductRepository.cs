using Microsoft.EntityFrameworkCore;
using Redis.Domain.AggregatesModel.ProductAggregate;
using Redis.Domain.Seedwork;

namespace Redis.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductContext _context;

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _context;
        }
    }

    public ProductRepository(ProductContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Product Add(Product product)
    {
        return  _context.Products.Add(product).Entity;

    }

    public void Update(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }
}