using System.Reflection;
using StackExchange.Redis;

namespace Redis.API.Infrastructure.Helpers;

public static class RedisHelper
{
    public static NameValueEntry[] ToNameValueEntries<T>(T obj)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var entries = new List<NameValueEntry>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj)?.ToString();
            entries.Add(new NameValueEntry(prop.Name, value));
        }

        return entries.ToArray();
    }
}