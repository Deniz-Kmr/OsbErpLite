using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Employees.Commands;
using OsbErpLite.Application.HumanResources.Employees.Dtos;
using OsbErpLite.Application.HumanResources.Employees.Queries;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Employee management within the HR module.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new employee record.
    /// </summary>
    /// <param name="dto">Employee details including name, national ID (TC Kimlik No), department, position, and salary.</param>
    /// <returns>The newly created employee.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        var command = new CreateEmployeeCommand(
            dto.FullName, dto.NationalId, dto.DepartmentId, dto.PositionId,
            dto.ManagerId, dto.HireDate, dto.Salary);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Calisan olusturuldu. Id: {EmployeeId}, Ad: {EmployeeName}, Departman: {DepartmentId}, Kullanici: {User}",
            result.Id, result.FullName, result.DepartmentId, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>
    /// Updates an existing employee record.
    /// </summary>
    /// <param name="id">The employee's unique identifier.</param>
    /// <param name="dto">Updated fields including status transitions.</param>
    /// <returns>The updated employee.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto dto)
    {
        var command = new UpdateEmployeeCommand(
            id, dto.FullName, dto.NationalId, dto.DepartmentId, dto.PositionId,
            dto.ManagerId, dto.HireDate, dto.Salary, dto.Status);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Calisan guncellendi. Id: {EmployeeId}, Ad: {EmployeeName}, Kullanici: {User}",
            result.Id, result.FullName, User.Identity?.Name);
        return Ok(ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>
    /// Soft-deletes an employee. Blocked if the employee has active leave requests or subordinates.
    /// </summary>
    /// <param name="id">The employee's unique identifier.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        _logger.LogInformation("Calisan silindi (soft). Id: {EmployeeId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    /// <summary>
    /// Retrieves an employee by unique identifier.
    /// </summary>
    /// <param name="id">The employee's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<EmployeeDto>.Fail($"Employee with Id '{id}' was not found."));
        return Ok(ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>
    /// Lists all active employees with pagination and optional department/position filters.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    /// <param name="departmentId">Optional department filter.</param>
    /// <param name="positionId">Optional position filter.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] Guid? positionId = null)
    {
        var result = await _mediator.Send(new GetAllEmployeesQuery(pageNumber, pageSize, departmentId, positionId));
        return Ok(ApiResponse<PaginatedList<EmployeeDto>>.Ok(result));
    }
}
