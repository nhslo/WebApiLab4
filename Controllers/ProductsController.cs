using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApiLab4.DTOs;
using WebApiLab4.Models;
using WebApiLab4.Repositories;
using WebApiLab4.Results;

namespace WebApiLab4.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(
    IProductRepository repository,
    IMapper mapper,
    ILogger<ProductsController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ReturnResult<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReturnResult<IEnumerable<ProductDto>>>> GetAll()
    {
        logger.LogInformation("Request received: list all products");
        var products = await repository.GetAllAsync();
        return Ok(ReturnResult<IEnumerable<ProductDto>>.Success(mapper.Map<IEnumerable<ProductDto>>(products)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReturnResult<ProductDto>>> Get(int id)
    {
        var product = await repository.GetAsync(id);
        if (product is null)
        {
            logger.LogWarning("Product with ID {ProductId} was not found", id);
            return NotFound(ReturnResult<object>.Failure($"Товар с ID {id} не найден."));
        }

        logger.LogInformation("Product with ID {ProductId} retrieved", id);
        return Ok(ReturnResult<ProductDto>.Success(mapper.Map<ProductDto>(product)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReturnResult<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReturnResult<ProductDto>>> Create(ProductInputDto dto)
    {
        var product = mapper.Map<Product>(dto);
        await repository.CreateAsync(product);
        logger.LogInformation("Product with ID {ProductId} created", product.Id);
        var result = ReturnResult<ProductDto>.Success(mapper.Map<ProductDto>(product));
        return CreatedAtAction(nameof(Get), new { id = product.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReturnResult<ProductDto>>> Update(int id, ProductInputDto dto)
    {
        var product = await repository.GetAsync(id);
        if (product is null)
        {
            logger.LogWarning("Product with ID {ProductId} was not found for update", id);
            return NotFound(ReturnResult<object>.Failure($"Товар с ID {id} не найден."));
        }

        mapper.Map(dto, product);
        await repository.UpdateAsync(product);
        logger.LogInformation("Product with ID {ProductId} updated", id);
        return Ok(ReturnResult<ProductDto>.Success(mapper.Map<ProductDto>(product)));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReturnResult<object>>> Delete(int id)
    {
        if (await repository.GetAsync(id) is null)
        {
            logger.LogWarning("Product with ID {ProductId} was not found for deletion", id);
            return NotFound(ReturnResult<object>.Failure($"Товар с ID {id} не найден."));
        }

        await repository.DeleteAsync(id);
        logger.LogInformation("Product with ID {ProductId} deleted", id);
        return Ok(ReturnResult<object>.Success(new { id }));
    }
}
