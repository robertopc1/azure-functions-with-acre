namespace Redis.Functions.Shared;

public class Common
{
    public const string RedisConnectionString = "ConnectionStrings:RedisConnectionString";
    public const string SQLConnectionString = "ConnectionStrings:SQLConnectionString";
    public const string SubscriptionChannel = "__keyevent@0__:json.set";
    public const string StoreProcedureUpdate = "[aidemo].spStyles_Update";
    public const string TableName = "[aidemo].[styles]";
    public const string Key = "wb-stream";
    
    public class ChannelMessage
    {
        public string SubscriptionChannel { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }

    public class StreamMessage
    {
        public string Id { get; set; }
        public Product Values { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
    }
}