using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.BusinessPartners.Queries;
using OsbErpLite.Application.Common.Models;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Manages business partners (customers, suppliers, and dual-role partners).
/// </summary>
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

    // The Global ExceptionHandlingMiddleware already automatically generates 400/401/403/404 responses.
    // I added ProducesResponseType attributes here to provide examples in the Swagger UI
    /// <summary>
    /// Creates a new business partner.
    /// </summary>
    /// <param name="command">Business partner details including name, tax number, and type.</param>
    /// <returns>The newly created business partner.</returns>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateBusinessPartnerCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    /// <summary>
    /// Updates an existing business partner.
    /// </summary>
    /// <param name="id">The business partner's unique identifier.</param>
    /// <param name="dto">Updated fields.</param>
    /// <returns>The updated business partner.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusinessPartnerDto dto)
    {
        var command = new UpdateBusinessPartnerCommand(id, dto.Name, dto.TaxNumber, dto.Type, dto.Email, dto.Phone, dto.Address, dto.City);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    /// <summary>
    /// Soft-deletes a business partner. Blocked if linked to active orders.
    /// </summary>
    /// <param name="id">The business partner's unique identifier.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBusinessPartnerCommand(id));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    /// <summary>
    /// Retrieves a business partner by unique identifier.
    /// </summary>
    /// <param name="id">The business partner's unique identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessPartnerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBusinessPartnerByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<BusinessPartnerDto>.Fail($"BusinessPartner with Id '{id}' was not found."));

        return Ok(ApiResponse<BusinessPartnerDto>.Ok(result));
    }

    /// <summary>
    /// Lists all active business partners with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BusinessPartnerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllBusinessPartnersQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<BusinessPartnerDto>>.Ok(result));
    }
}
