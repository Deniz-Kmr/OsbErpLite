using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.StockMovements.Commands;
using OsbErpLite.Application.StockMovements.Dtos;
using OsbErpLite.Application.StockMovements.Queries;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Stock movement tracking (manual entries and order-triggered movements).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StockMovementsController> _logger;

    public StockMovementsController(IMediator mediator, ILogger<StockMovementsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a manual stock movement (In or Out).
    /// </summary>
    /// <param name="command">Product, movement type, quantity, and warehouse.</param>
    /// <returns>The created stock movement record.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateStockMovementCommand command)
    {
        var result = await _mediator.Send(command);
        _logger.LogInformation(
            "Stok hareketi olusturuldu. Id: {MovementId}, Urun: {ProductId}, Tip: {MovementType}, Miktar: {Quantity}, Kullanici: {User}",
            result.Id, result.ProductId, result.MovementType, result.Quantity, User.Identity?.Name);
        return CreatedAtAction(nameof(GetByProductId), new { productId = result.ProductId }, ApiResponse<StockMovementDto>.Ok(result));
    }

    /// <summary>
    /// Lists stock movements for a specific product with pagination.
    /// </summary>
    /// <param name="productId">The product's unique identifier.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<StockMovementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProductId(Guid productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetStockMovementsByProductIdQuery(productId, pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<StockMovementDto>>.Ok(result));
    }
}
