using Redis.OM.Modeling;

namespace Redis.Functions.DataAccess.Models;

[Document(StorageType = StorageType.Json, 
    IndexName = "idx-styles", 
    Prefixes = ["Redis.IngestExample.Styles"])]
public class Styles
{
    [Indexed]
    [RedisIdField]
    public int id { get; set; }
    [Indexed]
    public string gender { get; set; }
    [Indexed]
    public string masterCategory { get; set; }
    [Indexed]
    public string subCategory { get; set; }
    [Indexed]
    public string articleType { get; set; }
    [Indexed]
    public string baseColour { get; set; }
    [Indexed]
    public string season { get; set; }
    [Indexed]
    public int year { get; set; }
    [Indexed]
    public string usage { get; set; }
    [Searchable]
    public string productDisplayName { get; set; }
}