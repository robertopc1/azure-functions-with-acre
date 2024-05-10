using DataAccess.DbAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        string redisConnectionString = configuration["RedisConnectionString"];
        
        var redisMultiplexer = ConnectionMultiplexer.Connect($"{redisConnectionString}"); // Replace with your Redis server connection details
        var provider = new RedisConnectionProvider(redisMultiplexer);
        var connection = provider.Connection;
        
        // Create index
        connection.CreateIndex(typeof(Styles));

        // Add services to the container.
        services.AddSingleton<IRedisConnectionProvider>(provider);
        services.AddSingleton<ISQLDataAccess, SQLDataAccess>();
        services.AddSingleton<IStylesData, StylesData>();
    })
    .Build();

host.Run();