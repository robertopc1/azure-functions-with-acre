using Redis.Domain.Seedwork;

namespace Redis.Domain.AggregatesModel.ProductAggregate;

public interface IProductRepository : IRepository<Product>
{
    Product Add(Product product);

    void Update(Product product);
}