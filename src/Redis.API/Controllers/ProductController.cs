using MediatR;
using Microsoft.AspNetCore.Mvc;
using Redis.API.Application.Commands;
using Redis.API.Application.Queries;
using System.Net;
using Redis.API.Application.Queries.DTOs;

namespace Redis.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return Ok(productId);
    }
    
    [Route("{productId}")]
    [HttpGet]
    [ProducesResponseType(typeof(RedisProductDTO),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetProduct(int productId)
    {
        try
        {
            var product = await _mediator.Send(new GetProductByIdQuery(productId));
            
            if (product == null)
                return NotFound();

            
            return Ok(product);
        }
        catch
        {
            return NotFound();
        }
    }
}
