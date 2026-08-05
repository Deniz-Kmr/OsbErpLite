using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Positions.Commands;
using OsbErpLite.Application.HumanResources.Positions.Dtos;
using OsbErpLite.Application.HumanResources.Positions.Queries;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(IMediator mediator, ILogger<PositionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PositionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePositionDto dto)
    {
        var command = new CreatePositionCommand(dto.Title, dto.Level);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Pozisyon olusturuldu. Id: {PositionId}, Baslik: {PositionTitle}, Seviye: {Level}, Kullanici: {User}",
            result.Id, result.Title, result.Level, User.Identity?.Name);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PositionDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<PositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionDto dto)
    {
        var command = new UpdatePositionCommand(id, dto.Title, dto.Level);
        var result = await _mediator.Send(command);
        _logger.LogInformation("Pozisyon guncellendi. Id: {PositionId}, Baslik: {PositionTitle}, Kullanici: {User}",
            result.Id, result.Title, User.Identity?.Name);
        return Ok(ApiResponse<PositionDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeletePositionCommand(id));
        _logger.LogInformation("Pozisyon silindi (soft). Id: {PositionId}, Kullanici: {User}",
            id, User.Identity?.Name);
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPositionByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<PositionDto>.Fail($"Position with Id '{id}' was not found."));
        return Ok(ApiResponse<PositionDto>.Ok(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PositionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllPositionsQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<PositionDto>>.Ok(result));
    }
}
