using Redis.OM.Modeling;

namespace Redis.API.Application.Queries.DTOs;

public class RedisProductDTO
{
    public int Id { get; set; }
    public string Gender { get; set; }
    public string MasterCategory { get; set; }
    public string SubCategory { get; set; }
    public string ArticleType { get; set; }
    public string BaseColour { get; set; }
    public string Season { get; set; }
    public string Year { get; set; }
    public string Usage { get; set; }
    public string ProductDisplayName { get; set; }
}