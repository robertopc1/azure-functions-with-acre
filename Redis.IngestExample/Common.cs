namespace Redis.IngestExample;

public class Common
{
    public const string ConnectionString = "RedisConnectionString";
    public const string SubscriptionChannel = "__keyevent@0__:json.set";
    
    public class ChannelMessage
    {
        public string SubscriptionChannel { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}