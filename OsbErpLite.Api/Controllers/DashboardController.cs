using MediatR;
using OsbErpLite.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Application.Dashboard.Queries;

namespace OsbErpLite.Api.Controllers;

/// <summary>
/// Dashboard statistics and summary data for the ERP system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Returns a summary of key metrics across all modules.
    /// TotalStockValue is only visible to Admin and Manager roles.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var includeFinancial = User.IsInRole("Admin") || User.IsInRole("Manager");
        var result = await _mediator.Send(new GetDashboardSummaryQuery(includeFinancial));

        _logger.LogInformation("Dashboard ozeti goruntulendi. Kullanici: {User}, FinansalVeriDahil: {IncludeFinancial}",
            User.Identity?.Name, includeFinancial);

        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
    }

    /// <summary>
    /// Returns monthly stock movement totals (In/Out) for the last 6 months. Admin and Manager only.
    /// </summary>
    [HttpGet("stock-movements")]
    [Authorize(Roles = Roles.AdminOrManager)]
    [ProducesResponseType(typeof(ApiResponse<List<MonthlyStockMovementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockMovements()
    {
        var result = await _mediator.Send(new GetMonthlyStockMovementSummaryQuery());

        _logger.LogInformation("Aylik stok hareket istatistikleri goruntulendi. Kullanici: {User}",
            User.Identity?.Name);

        return Ok(ApiResponse<List<MonthlyStockMovementDto>>.Ok(result));
    }

    /// <summary>
    /// Returns the number of active employees per department.
    /// </summary>
    [HttpGet("department-distribution")]
    [ProducesResponseType(typeof(ApiResponse<List<DepartmentDistributionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentDistribution()
    {
        var result = await _mediator.Send(new GetDepartmentEmployeeDistributionQuery());

        _logger.LogInformation("Departman calisan dagilimi goruntulendi. Kullanici: {User}",
            User.Identity?.Name);

        return Ok(ApiResponse<List<DepartmentDistributionDto>>.Ok(result));
    }
}
