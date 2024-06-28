using MediatR;
using System.Runtime.Serialization;
using Redis.API.Application.Queries.DTOs;

namespace Redis.API.Application.Queries;

[DataContract]
public class GetProductByIdQuery : IRequest<RedisProductDTO>
{
    [DataMember]
    public int Id { get; private set; }
    
    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}