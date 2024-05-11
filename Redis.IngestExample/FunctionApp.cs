using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Redis;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;
using Redis.OM.Searching;


namespace Redis.IngestExample;

public class FunctionApp
{
    private readonly ILogger _logger;
    private readonly IRedisCollection<Styles> _stylesCollection;
    private readonly IRedisConnectionProvider _connectionProvider;
    private readonly IStylesData _data;
    public FunctionApp(ILoggerFactory loggerFactory,
           IRedisConnectionProvider connectionProvider,
           IStylesData stylesData
    )
    {
        _logger = loggerFactory.CreateLogger<FunctionApp>();
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _stylesCollection = _connectionProvider.RedisCollection<Styles>();
        _data = stylesData ?? throw new ArgumentNullException(nameof(stylesData));
    }
    
    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("IngestWithOmExample")]
    public async Task Run(
        [SqlTrigger("[aidemo].[styles]", "SQLConnectionString")] 
        IReadOnlyList<SqlChange<Styles>> changes) 
    {
        IEnumerable<Styles> changeList = changes.Select<SqlChange<Styles>, Styles>(change => change.Item);
        await _stylesCollection.UpdateAsync(changeList);
    }
    
    // The RedisInput binding only supports single output commands at the moment
    //https://github.com/Azure/azure-functions-redis-extension/blob/main/src/Microsoft.Azure.WebJobs.Extensions.Redis/Bindings/RedisAsyncConverter.cs#L63
    // since in this example we are working with json documents, we will use OM at the moment. 
    // [Function(nameof(WriteBehind))]
    // public async Task WriteBehind(
    // [RedisPubSubTrigger(Common.ConnectionString, Common.SubscriptionChannel)] Common.ChannelMessage channelMessage)
    // {
    //     var key = channelMessage.Message; //The name of the key that was set
    //     var value = await _stylesCollection.FindByIdAsync(key);
    //     
    //     if(value != null)
    //         // Update the row in SQL
    //         await _data.UpdateStyle(value);
    //     
    //     //Log the time that the function was executed.
    //     _logger.LogInformation($"C# Redis trigger function executed at: {DateTime.Now}");
    // }
    
    
    //*** WIP ****//
    // [Function("IngestWithInputBiding")]
    // [RedisOutput("RedisConnectionString", "JSON.MSET")]
    // public string Run(
    //     [SqlTrigger("[aidemo].[styles]", "SQLConnectionString")]
    //     IReadOnlyList<SqlChange<Styles>> changes)
    // {
    //     IEnumerable<Styles> changeList = changes.Select<SqlChange<Styles>, Styles>(change => change.Item);
    //
    //     StringBuilder jsonStringCollection = new StringBuilder();
    //     
    //     foreach (var item in changeList)
    //     {
    //         var jsonString = JsonSerializer.Serialize(item);
    //         jsonStringCollection.Append($"Redis.IngestExample.Styles:{item.id} $ {jsonString}");
    //     }
    //     
    //     return $"{jsonStringCollection}";
    // }
    
    
}

