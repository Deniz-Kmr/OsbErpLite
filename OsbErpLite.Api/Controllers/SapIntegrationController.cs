using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OsbErpLite.Api.Common;
using OsbErpLite.Api.Extensions;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Dtos;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/sap")]
[Authorize]
public class SapIntegrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SapIntegrationController> _logger;

    public SapIntegrationController(IMediator mediator, ILogger<SapIntegrationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("sync-business-partners")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting(RateLimitingServiceExtensions.SapSyncPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<SyncResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncBusinessPartners()
    {
        _logger.LogInformation(
            "SAP Business Partner sync tetiklendi. Kullanici: {User}", User.Identity?.Name);

        var result = await _mediator.Send(new SyncBusinessPartnersFromSapCommand());

        _logger.LogInformation(
            "SAP sync tamamlandi. Eklenen: {Added}, Guncellenen: {Updated}, Hata: {Errors}, Kullanici: {User}",
            result.AddedCount, result.UpdatedCount, result.Errors.Count, User.Identity?.Name);

        return Ok(ApiResponse<SyncResultDto>.Ok(result, "SAP sync islemi tamamlandi."));
    }
}
