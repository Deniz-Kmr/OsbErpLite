using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Warehouses.Commands;
using OsbErpLite.Application.Warehouses.Dtos;
using OsbErpLite.Application.Warehouses.Queries;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Warehouse management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WarehousesController> _logger;

    public WarehousesController(IMediator mediator, ILogger<WarehousesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new warehouse.
    /// </summary>
    /// <param name="command">Warehouse name and location.</param>
    /// <returns>The newly created warehouse.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
    {
        var result = await _mediator.Send(command);
        _logger.LogInformation("Depo olusturuldu. Id: {WarehouseId}, Ad: {WarehouseName}, Kullanici: {User}",
            result.Id, result.Name, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<WarehouseDto>.Ok(result));
    }

    /// <summary>
    /// Updates an existing warehouse.
    /// </summary>
    /// <param name="id">The warehouse's unique identifier.</param>
    /// <param name="dto">Updated name and/or location.</param>
    /// <returns>The updated warehouse.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseDto dto)
    {
        var command = new UpdateWarehouseCommand(id, dto.Name, dto.Location);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Depo guncellendi. Id: {WarehouseId}, Ad: {WarehouseName}, Kullanici: {User}",
            id, result.Name, User.Identity?.Name);
        return Ok(ApiResponse<WarehouseDto>.Ok(result));
    }

    /// <summary>
    /// Soft-deletes a warehouse. Blocked if it contains active products.
    /// </summary>
    /// <param name="id">The warehouse's unique identifier.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteWarehouseCommand(id));
        _logger.LogInformation("Depo silindi (soft delete). Id: {WarehouseId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    /// <summary>
    /// Retrieves a warehouse by unique identifier.
    /// </summary>
    /// <param name="id">The warehouse's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetWarehouseByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<WarehouseDto>.Fail($"Warehouse with Id '{id}' was not found."));

        return Ok(ApiResponse<WarehouseDto>.Ok(result));
    }

    /// <summary>
    /// Lists all active warehouses with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<WarehouseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllWarehousesQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<WarehouseDto>>.Ok(result));
    }
}
