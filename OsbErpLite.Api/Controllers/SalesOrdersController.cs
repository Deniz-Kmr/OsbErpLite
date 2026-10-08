using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.SalesOrders.Commands;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Application.SalesOrders.Queries;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Sales order lifecycle management (create, approve, reject, complete).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesOrdersController> _logger;

    public SalesOrdersController(IMediator mediator, ILogger<SalesOrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new sales order in Draft status.
    /// </summary>
    /// <param name="dto">Customer, order date, and order lines with product/quantity/price.</param>
    /// <returns>The newly created sales order.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.All)]
    [ProducesResponseType(typeof(ApiResponse<SalesOrderDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderDto dto)
    {
        var command = new CreateSalesOrderCommand(dto.CustomerId, dto.OrderDate, dto.Lines);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Satis siparisi olusturuldu. Id: {OrderId}, Musteri: {CustomerId}, Toplam: {TotalAmount}, Kullanici: {User}",
            result.Id, result.CustomerId, result.TotalAmount, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SalesOrderDto>.Ok(result));
    }

    /// <summary>
    /// Retrieves a sales order by unique identifier.
    /// </summary>
    /// <param name="id">The sales order's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SalesOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSalesOrderByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<SalesOrderDto>.Fail($"SalesOrder with Id '{id}' was not found."));
        return Ok(ApiResponse<SalesOrderDto>.Ok(result));
    }

    /// <summary>
    /// Lists all sales orders with pagination and optional status filter.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    /// <param name="status">Optional order status filter.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<SalesOrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        var result = await _mediator.Send(new GetAllSalesOrdersQuery(pageNumber, pageSize, status));
        return Ok(ApiResponse<PaginatedList<SalesOrderDto>>.Ok(result));
    }

    /// <summary>
    /// Approves a sales order. Transitions status from Draft/Pending to Approved.
    /// </summary>
    /// <param name="id">The sales order's unique identifier.</param>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<SalesOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<SalesOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new ApproveSalesOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satis siparisi onaylandi. OrderId: {OrderId}, OnaylayanEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<SalesOrderDto>.Ok(result));
    }

    /// <summary>
    /// Rejects a sales order. Transitions status to Rejected.
    /// </summary>
    /// <param name="id">The sales order's unique identifier.</param>
    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<SalesOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<SalesOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new RejectSalesOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satis siparisi reddedildi. OrderId: {OrderId}, ReddedenEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<SalesOrderDto>.Ok(result));
    }

    /// <summary>
    /// Completes a sales order. Triggers automatic stock-out movement for all order lines.
    /// </summary>
    /// <param name="id">The sales order's unique identifier.</param>
    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<SalesOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<SalesOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new CompleteSalesOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satis siparisi tamamlandi. OrderId: {OrderId}, TamamlayanEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<SalesOrderDto>.Ok(result));
    }

    private Guid? GetEmployeeIdFromClaims()
    {
        var claim = User.FindFirst("EmployeeId");
        return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
