using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Redis.API.Infrastructure.ServiceExtensions;
using Redis.Domain.AggregatesModel.ProductAggregate;
using Redis.Infrastructure;

namespace Redis.API.Infrastructure;

public class ProductContextSeed
{
      public  async Task SeedAsync(ProductContext context, 
          IWebHostEnvironment env,
          ILogger<ProductContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ProductContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;
                
                if (string.IsNullOrEmpty(contentRootPath))
                {
                    throw new InvalidOperationException("Content root path is not set.");
                }
                
                var parentDirectory = Directory.GetParent(contentRootPath)?.Parent?.FullName;

                if (string.IsNullOrEmpty(parentDirectory))
                {
                    //We are running in docker
                    parentDirectory = contentRootPath;
                }

                await context.Database.EnsureCreatedAsync();
                
                context.Database.Migrate();

                if (!context.Products.Any())
                {
                    // Create an execution strategy
                    var strategy = context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = await context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT aidemo.styles ON");

                                context.Products.AddRange(GetProductsFromFile(parentDirectory, logger));

                                await context.SaveChangesAsync();

                                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT aidemo.styles OFF");
                                await transaction.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                // Rollback transaction if any error occurs
                                logger.LogError(ex, "An error occurred, rolling back transaction");
                                await transaction.RollbackAsync();

                                // Ensure IDENTITY_INSERT is turned off in case of an exception
                                logger.LogInformation(
                                    "Disabling IDENTITY_INSERT for table aidemo.styles after exception");
                                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT aidemo.styles OFF");

                                throw;
                            }
                        }
                    });
                }
                
            });
        }

        private IEnumerable<Product> GetProductsFromFile(string contentRootPath, ILogger<ProductContextSeed> log)
        {
            Console.WriteLine($"Product File------> {contentRootPath}");
            
            string csvProducts = Path.Combine(contentRootPath, "infra/sql", "styles.csv");
            
            if (!File.Exists(csvProducts))
            {
                return null;
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "id", "articletype", "basecolour", "gender", "mastercategory", "productdisplayname", "season", "subcategory", "usage", "year" };
                csvheaders = GetHeaders(requiredHeaders, csvProducts);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return null;
            }

            return File.ReadAllLines(csvProducts)
                                        .Skip(1) // skip header column
                                        .SelectTry(x => CreateProduct(x))
                                        .OnCaughtException(ex => { log.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private Product CreateProduct(string value)
        {
            var values = value.Split(",");
            
            if (values[0] == "0")
            {
                throw new Exception("Product is null or empty");
            }

            return new Product(int.Parse(values[0]),
                            values[1],
                            values[2], 
                            values[3],
                            values[4],
                            values[5],
                            values[6], 
                            int.Parse(values[7]),
                            values[8],
                            values[9]);
        }
        
        private string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

     
        private AsyncRetryPolicy CreatePolicy( ILogger<ProductContextSeed> logger, string prefix, int retries =3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
}