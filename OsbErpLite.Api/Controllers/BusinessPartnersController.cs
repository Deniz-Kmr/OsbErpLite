using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.BusinessPartners.Queries;
using OsbErpLite.Application.Common.Models;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BusinessPartnersController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessPartnersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBusinessPartnerCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusinessPartnerDto dto)
    {
        var command = new UpdateBusinessPartnerCommand(id, dto.Name, dto.TaxNumber, dto.Type, dto.Email, dto.Phone, dto.Address, dto.City);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBusinessPartnerCommand(id));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBusinessPartnerByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<BusinessPartnerDto>.Fail($"BusinessPartner with Id '{id}' was not found."));

        return Ok(ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BusinessPartnerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllBusinessPartnersQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<BusinessPartnerDto>>.Ok(result));
    }
}
