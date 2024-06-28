using AutoMapper;
using Redis.API.Application.Queries;
using Redis.API.Application.Queries.DTOs;

namespace Redis.API.Application.Mappings;


public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<RedisProduct, RedisProductDTO>();
    }
}