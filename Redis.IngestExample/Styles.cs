using Redis.OM.Modeling;

namespace Redis.IngestExample;

//[Document(StorageType = StorageType.Json, IndexName = "idx-styles")]
public class Styles
{
    // [Indexed]
    // [RedisIdField]
    // public int id { get; set; }
    // public string gender { get; set; }
    // public string masterCategory { get; set; }
    // public string subCategory { get; set; }
    // public string articleType { get; set; }
    // public string baseColour { get; set; }
    // public string season { get; set; }
    // public int year { get; set; }
    // public string usage { get; set; }
    // [Searchable]
    // public string productDisplayName { get; set; }
}