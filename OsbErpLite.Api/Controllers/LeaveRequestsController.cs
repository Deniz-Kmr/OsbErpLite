using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.LeaveRequests.Commands;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Application.HumanResources.LeaveRequests.Queries;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Leave request management with approval workflow (Pending → Approved/Rejected).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LeaveRequestsController> _logger;

    public LeaveRequestsController(IMediator mediator, ILogger<LeaveRequestsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new leave request in Pending status.
    /// </summary>
    /// <param name="dto">Employee, date range, and reason for the leave.</param>
    /// <returns>The newly created leave request.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LeaveRequestDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
    {
        var command = new CreateLeaveRequestCommand(dto.EmployeeId, dto.StartDate, dto.EndDate, dto.Reason);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Izin talebi olusturuldu. Id: {LeaveRequestId}, Calisan: {EmployeeId}, Baslangic: {StartDate}, Bitis: {EndDate}, Kullanici: {User}",
            result.Id, result.EmployeeId, result.StartDate, result.EndDate, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<LeaveRequestDto>.Ok(result));
    }

    /// <summary>
    /// Retrieves a leave request by unique identifier.
    /// </summary>
    /// <param name="id">The leave request's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetLeaveRequestByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<LeaveRequestDto>.Fail($"LeaveRequest with Id '{id}' was not found."));
        return Ok(ApiResponse<LeaveRequestDto>.Ok(result));
    }

    /// <summary>
    /// Lists leave requests with pagination and optional employee/status filters.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    /// <param name="employeeId">Optional employee filter.</param>
    /// <param name="status">Optional status filter (Pending, Approved, Rejected).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<LeaveRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] LeaveRequestStatus? status = null)
    {
        var result = await _mediator.Send(new GetAllLeaveRequestsQuery(pageNumber, pageSize, employeeId, status));
        return Ok(ApiResponse<PaginatedList<LeaveRequestDto>>.Ok(result));
    }

    /// <summary>
    /// Approves a pending leave request.
    /// </summary>
    /// <param name="id">The leave request's unique identifier.</param>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<LeaveRequestDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new ApproveLeaveRequestCommand(id, employeeId.Value));
        _logger.LogInformation("Izin talebi onaylandi. LeaveRequestId: {LeaveRequestId}, OnaylayanEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<LeaveRequestDto>.Ok(result));
    }

    /// <summary>
    /// Rejects a pending leave request.
    /// </summary>
    /// <param name="id">The leave request's unique identifier.</param>
    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var employeeId = GetEmployeeIdFromClaims();
        if (employeeId is null)
            return BadRequest(ApiResponse<LeaveRequestDto>.Fail("Kullanicinin bagli bir Employee kaydi bulunamadi."));

        var result = await _mediator.Send(new RejectLeaveRequestCommand(id, employeeId.Value));
        _logger.LogInformation("Izin talebi reddedildi. LeaveRequestId: {LeaveRequestId}, ReddedenEmployee: {EmployeeId}, Kullanici: {User}",
            id, employeeId.Value, User.Identity?.Name);
        return Ok(ApiResponse<LeaveRequestDto>.Ok(result));
    }

    private Guid? GetEmployeeIdFromClaims()
    {
        var claim = User.FindFirst("EmployeeId");
        return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
