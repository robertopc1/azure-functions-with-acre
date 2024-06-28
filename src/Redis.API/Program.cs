using Redis.API;
using Redis.API.Infrastructure;
using Redis.API.Infrastructure.Extensions;
using Redis.Infrastructure;


public class Program
{
    public static int Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        try
        {
            host.MigrateDbContext<ProductContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>();
                var logger = services.GetService<ILogger<ProductContextSeed>>();
                
                new ProductContextSeed()
                    .SeedAsync(context, env, logger)
                    .Wait();
            });

            host.Run();

            return 0;
        }
        catch (Exception ex)
        {
            return 1;
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) 
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}

