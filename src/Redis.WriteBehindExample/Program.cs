using Redis.Functions.DataAccess.DbAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.Functions.Shared;
using StackExchange.Redis;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    { 
        IConfiguration configuration = hostContext.Configuration;
        string redisConnectionString = configuration[Common.RedisConnectionString] ?? throw new ArgumentNullException(nameof(configuration));
        
        var redisMultiplexer = ConnectionMultiplexer.Connect($"{redisConnectionString}"); // Replace with your Redis server connection details
        var provider = new RedisConnectionProvider(redisMultiplexer);
        var connection = provider.Connection;
        
        // Create index
        connection.CreateIndex(typeof(Styles));

        // Add services to the container.
        services.AddSingleton<IRedisConnectionProvider>(provider);
        services.AddSingleton<ISQLDataAccess, SQLDataAccess>();
        services.AddSingleton<IStylesData, StylesData>();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();