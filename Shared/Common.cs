namespace Shared;

public class Common
{
    public const string RedisConnectionString = "ConnectionStrings:RedisConnectionString";
    public const string SQLConnectionString = "ConnectionStrings:SQLConnectionString";
    public const string SubscriptionChannel = "__keyevent@0__:json.set";
    public const string StoreProcedureUpdate = "[aidemo].spStyles_Update";
    public const string TableName = "[aidemo].[styles]";
    
    public class ChannelMessage
    {
        public string SubscriptionChannel { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}