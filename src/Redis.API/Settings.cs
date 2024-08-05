namespace Redis.API;

public class Settings
{
    public bool UseWriteBehind { get; set; }
    public string WriteBehindStreamName { get; set; }
}