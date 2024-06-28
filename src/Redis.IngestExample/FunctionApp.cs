using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.Functions.Shared;


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
        [SqlTrigger(Common.TableName, Common.SQLConnectionString)] 
        IReadOnlyList<SqlChange<Styles>> changes) 
    {
        _logger.LogInformation($"Entered the Azure Function --- {DateTime.Now}");
        
        IEnumerable<Styles> changeList = changes    
            .Select<SqlChange<Styles>, Styles>(change => change.Item);

        if (changeList.Any())
            await _stylesCollection.InsertAsync(changeList);
        
        _logger.LogInformation($"Finished updating {DateTime.Now}");
    }
    
    //*** WIP ****//
    // JSON.MSET is supported in RedisJson 2.6 ACRE is currently in 2.4.
    // Since we can't use JSON.MSET to send all the keys that have been updated in SQL
    // We will update the code once ACRE gets upgraded to a later version
    // [Function("IngestWithInputBiding")]
    // [RedisOutput(Common.RedisConnectionString, "JSON.SET")]
    // public string Run(
    //     [SqlTrigger("[aidemo].[styles]", Common.SQLConnectionString)]
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

