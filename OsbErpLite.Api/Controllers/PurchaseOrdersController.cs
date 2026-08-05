using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.PurchaseOrders.Commands;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Application.PurchaseOrders.Queries;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PurchaseOrdersController> _logger;

    public PurchaseOrdersController(IMediator mediator, ILogger<PurchaseOrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Employee")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
    {
        var command = new CreatePurchaseOrderCommand(dto.SupplierId, dto.OrderDate, dto.Lines);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Satin alma siparisi olusturuldu. Id: {OrderId}, Tedarikci: {SupplierId}, Toplam: {TotalAmount}, Kullanici: {User}",
            result.Id, result.SupplierId, result.TotalAmount, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PurchaseOrderDto>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<PurchaseOrderDto>.Fail($"PurchaseOrder with Id '{id}' was not found."));
        return Ok(ApiResponse<PurchaseOrderDto>.Ok(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PurchaseOrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        var result = await _mediator.Send(new GetAllPurchaseOrdersQuery(pageNumber, pageSize, status));
        return Ok(ApiResponse<PaginatedList<PurchaseOrderDto>>.Ok(result));
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<PurchaseOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new ApprovePurchaseOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satin alma siparisi onaylandi. OrderId: {OrderId}, OnaylayanEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<PurchaseOrderDto>.Ok(result));
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<PurchaseOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new RejectPurchaseOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satin alma siparisi reddedildi. OrderId: {OrderId}, ReddedenEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<PurchaseOrderDto>.Ok(result));
    }

    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<PurchaseOrderDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new CompletePurchaseOrderCommand(id, employeeId.Value));
        _logger.LogInformation("Satin alma siparisi tamamlandi. OrderId: {OrderId}, TamamlayanEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<PurchaseOrderDto>.Ok(result));
    }

    private Guid? GetEmployeeIdFromClaims()
    {
        var claim = User.FindFirst("EmployeeId");
        return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
