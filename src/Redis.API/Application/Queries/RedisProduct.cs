using System.Text.Json.Serialization;
using Redis.OM.Modeling;

namespace Redis.API.Application.Queries;

[Document(StorageType = StorageType.Json,
    Prefixes = ["Redis.IngestExample.Styles"],
    IndexName= "idx-styles")]

public class RedisProduct
{
    [JsonPropertyName("id")]
    [RedisIdField]
    [Indexed (PropertyName = "id")]
    public int Id { get; set; }

    [JsonPropertyName("gender")]
    [Indexed(PropertyName = "gender")]
    public string Gender { get; set; }
    
    [JsonPropertyName("masterCategory")]
    public string MasterCategory { get; set; }
    
    [JsonPropertyName("subCategory")]
    public string SubCategory { get; set; }
    
    [JsonPropertyName("articleType")]
    public string ArticleType { get; set; }
    
    [JsonPropertyName("baseColour")]
    public string BaseColour { get; set; }
    
    [JsonPropertyName("season")]
    public string Season { get; set; }
    
    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    [JsonPropertyName("usage")]
    public string Usage { get; set; }
    
    [JsonPropertyName("productDisplayName")]
    [Searchable(PropertyName = "productDisplayName")]
    public string ProductDisplayName { get; set; }
    
}