using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Departments.Commands;
using OsbErpLite.Application.HumanResources.Departments.Dtos;
using OsbErpLite.Application.HumanResources.Departments.Queries;

namespace OsbErpLite.Api.Controllers;

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

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
    {
        var command = new CreateDepartmentCommand(dto.Name, dto.ManagerId);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Departman olusturuldu. Id: {DepartmentId}, Ad: {DepartmentName}, Kullanici: {User}",
            result.Id, result.Name, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<DepartmentDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
    {
        var command = new UpdateDepartmentCommand(id, dto.Name, dto.ManagerId);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Departman guncellendi. Id: {DepartmentId}, Ad: {DepartmentName}, Kullanici: {User}",
            result.Id, result.Name, User.Identity?.Name);
        return Ok(ApiResponse<DepartmentDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDepartmentCommand(id));
        _logger.LogInformation("Departman silindi (soft). Id: {DepartmentId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDepartmentByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<DepartmentDto>.Fail($"Department with Id '{id}' was not found."));
        return Ok(ApiResponse<DepartmentDto>.Ok(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<DepartmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllDepartmentsQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<DepartmentDto>>.Ok(result));
    }
}
