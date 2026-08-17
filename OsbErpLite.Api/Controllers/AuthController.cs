using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsbErpLite.Api.Common;
using OsbErpLite.Application.Common.Models;
using Microsoft.AspNetCore.RateLimiting;
using OsbErpLite.Api.Extensions;
using OsbErpLite.Application.Identity.Commands;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Application.Identity.Queries;

namespace OsbErpLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(result));
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    [HttpPost("refresh-token")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] RevokeRefreshTokenCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(null!, "Logged out successfully."));
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<UserDto>>.Ok(result));
    }

    [HttpGet("users/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<UserDto>.Fail($"User with Id '{id}' was not found."));

        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    [HttpPut("users/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = GetCurrentUserId();
        var command = new UpdateUserCommand(id, dto.Role, dto.IsActive, dto.EmployeeId, currentUserId);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    [HttpDelete("users/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        await _mediator.Send(new DeleteUserCommand(id, currentUserId));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPut("users/{id:guid}/reset-password")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting(RateLimitingServiceExtensions.PasswordChangePolicyName)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
    {
        var currentUserId = GetCurrentUserId();
        await _mediator.Send(new AdminResetPasswordCommand(id, dto.NewPassword, currentUserId));
        return Ok(ApiResponse<object>.Ok(null!, "Password has been reset successfully."));
    }

    [HttpPut("me/change-password")]
    [Authorize]
    [EnableRateLimiting(RateLimitingServiceExtensions.PasswordChangePolicyName)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangePasswordDto dto)
    {
        var currentUserId = GetCurrentUserId();
        await _mediator.Send(new ChangeOwnPasswordCommand(currentUserId, dto.CurrentPassword, dto.NewPassword));
        return Ok(ApiResponse<object>.Ok(null!, "Password has been changed successfully."));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identity could not be resolved.");
        return Guid.Parse(userIdClaim);
    }
}
