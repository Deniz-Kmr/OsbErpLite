using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Departments.Commands;
using OsbErpLite.Application.HumanResources.Departments.Dtos;
using OsbErpLite.Application.HumanResources.Departments.Queries;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Department management within the HR module.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(IMediator mediator, ILogger<DepartmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="dto">Department name and optional manager.</param>
    /// <returns>The newly created department.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
    {
        var command = new CreateDepartmentCommand(dto.Name, dto.ManagerId);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Departman olusturuldu. Id: {DepartmentId}, Ad: {DepartmentName}, Kullanici: {User}",
            result.Id, result.Name, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<DepartmentDto>.Ok(result));
    }

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    /// <param name="id">The department's unique identifier.</param>
    /// <param name="dto">Updated name and/or manager.</param>
    /// <returns>The updated department.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
    {
        var command = new UpdateDepartmentCommand(id, dto.Name, dto.ManagerId);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Departman guncellendi. Id: {DepartmentId}, Ad: {DepartmentName}, Kullanici: {User}",
            result.Id, result.Name, User.Identity?.Name);
        return Ok(ApiResponse<DepartmentDto>.Ok(result));
    }

    /// <summary>
    /// Soft-deletes a department. Blocked if it has active employees.
    /// </summary>
    /// <param name="id">The department's unique identifier.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDepartmentCommand(id));
        _logger.LogInformation("Departman silindi (soft). Id: {DepartmentId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    /// <summary>
    /// Retrieves a department by unique identifier.
    /// </summary>
    /// <param name="id">The department's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDepartmentByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<DepartmentDto>.Fail($"Department with Id '{id}' was not found."));
        return Ok(ApiResponse<DepartmentDto>.Ok(result));
    }

    /// <summary>
    /// Lists all active departments with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<DepartmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllDepartmentsQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<DepartmentDto>>.Ok(result));
    }
}
