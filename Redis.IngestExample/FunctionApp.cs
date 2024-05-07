using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Redis;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;


namespace Redis.IngestExample;

public class FunctionApp
{
    private readonly ILogger _logger;
    private readonly IRedisCollection<Styles> _stylesCollection;
    private readonly IRedisConnectionProvider _connectionProvider;
    private const string RedisConnectionSetting = "RedisConnectionString";

    public FunctionApp(ILoggerFactory loggerFactory, 
           IRedisConnectionProvider connectionProvider
            )
    {
        _logger = loggerFactory.CreateLogger<FunctionApp>();
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException();
        _stylesCollection = _connectionProvider.RedisCollection<Styles>();
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    // [Function("IngestWithOmExample")]
    // public async Task Run(
    //     [SqlTrigger("[aidemo].[styles]", "SQLConnectionString")] 
    //     IReadOnlyList<SqlChange<Styles>> changes) 
    // {
    //     IEnumerable<Styles> changeList = changes.Select<SqlChange<Styles>, Styles>(change => change.Item);
    //     await _stylesCollection.UpdateAsync(changeList);
    // }
    
    //**** Not Working ** //
    // [Function("IngestWithInputBiding")]
    // [RedisOutput("RedisConnectionString", "JSON.MSET")]
    // public string Run(
    //     [SqlTrigger("[aidemo].[styles]", "ConnectionString")]
    //     IReadOnlyList<SqlChange<Styles>> changes)
    // {
    //     IEnumerable<Styles> changeList = changes.Select<SqlChange<Styles>, Styles>(change => change.Item);
    //
    //     StringBuilder jsonStringCollection = new StringBuilder();
    //     
    //     foreach (var item in changeList)
    //     {
    //         var jsonString = JsonSerializer.Serialize(item);
    //         jsonStringCollection.Append($"Redis.IngestExample.Styles:{item.id} $ {item} ");
    //     }
    //     
    //     return $"${jsonStringCollection}";
    // }
    
    //**** Not Working ** //
    [Function(nameof(WriteBehind))]
    public void WriteBehind(
    [RedisPubSubTrigger(RedisConnectionSetting, "__keyevent@0__:set")] string newKey,
    [RedisInput("RedisConnectionString", "GET {Message}")] string setValue)
    {
        _logger.LogInformation($"My Value:{newKey}");
        
        // //var key = channelMessage.Message; //The name of the key that was set
        // //var value = 0.0;
        //
        // //Check if the value is a number. If not, log an error and return.
        // //if (double.TryParse(setValue, out double result))
        // {
        //     value = result; //The value that was set. (i.e. the price.)
        //     logger.LogInformation($"Key '{channelMessage.Message}' was set to value '{value}'");
        // }
        // else
        // {
        //     logger.LogInformation($"Invalid input for key '{key}'. A number is expected.");
        //     return;
        // }        
        //
        // // Define the name of the table you created and the column names.
        // String tableName = "dbo.inventory";
        // String column1Value = "ItemName";
        // String column2Value = "Price";        
        //
        // logger.LogInformation($" '{SQLAddress}'");
        // using (SqlConnection connection = new SqlConnection(SQLAddress))
        //     {
        //         connection.Open();
        //         using (SqlCommand command = new SqlCommand())
        //         {
        //             command.Connection = connection;
        //
        //             //Form the SQL query to update the database. In practice, you would want to use a parameterized query to prevent SQL injection attacks.
        //             //An example query would be something like "UPDATE dbo.inventory SET Price = 1.75 WHERE ItemName = 'Apple'".
        //             command.CommandText = "UPDATE " + tableName + " SET " + column2Value + " = " + value + " WHERE " + column1Value + " = '" + key + "'";
        //             int rowsAffected = command.ExecuteNonQuery(); //The query execution returns the number of rows affected by the query. If the key doesn't exist, it will return 0.
        //
        //             if (rowsAffected == 0) //If key doesn't exist, add it to the database
        //          {
        //                  //Form the SQL query to update the database. In practice, you would want to use a parameterized query to prevent SQL injection attacks.
        //                  //An example query would be something like "INSERT INTO dbo.inventory (ItemName, Price) VALUES ('Bread', '2.55')".
        //                 command.CommandText = "INSERT INTO " + tableName + " (" + column1Value + ", " + column2Value + ") VALUES ('" + key + "', '" + value + "')";
        //                 command.ExecuteNonQuery();
        //
        //                 logger.LogInformation($"Item " + key + " has been added to the database with price " + value + "");
        //             }
        //
        //             else {
        //                 logger.LogInformation($"Item " + key + " has been updated to price " + value + "");
        //             }
        //         }
        //         connection.Close();
        //     }
        //
        //     //Log the time that the function was executed.
        //     logger.LogInformation($"C# Redis trigger function executed at: {DateTime.Now}");
    }
}

