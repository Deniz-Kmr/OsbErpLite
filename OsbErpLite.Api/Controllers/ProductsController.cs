using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Products.Commands;
using OsbErpLite.Application.Products.Dtos;
using OsbErpLite.Application.Products.Queries;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        _logger.LogInformation("Urun olusturuldu. Id: {ProductId}, Ad: {ProductName}, SKU: {SKU}, Kullanici: {User}",
            result.Id, result.Name, result.SKU, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var command = new UpdateProductCommand(id, dto.Name, dto.SKU, dto.UnitPrice, dto.WarehouseId);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Urun guncellendi. Id: {ProductId}, Ad: {ProductName}, SKU: {SKU}, Kullanici: {User}",
            id, result.Name, result.SKU, User.Identity?.Name);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        _logger.LogInformation("Urun silindi (soft delete). Id: {ProductId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<ProductDto>.Fail($"Product with Id '{id}' was not found."));

        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? warehouseId = null)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(pageNumber, pageSize, warehouseId));
        return Ok(ApiResponse<PaginatedList<ProductDto>>.Ok(result));
    }
}
