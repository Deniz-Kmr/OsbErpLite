using System.Security.Claims;
using OsbErpLite.Domain.Common;
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

/// <summary>
/// Authentication and user management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="command">Registration details including username, email, and password.</param>
    /// <returns>The created user with JWT tokens.</returns>
    [HttpPost("register")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="command">Login credentials (username and password).</param>
    /// <returns>Access token, refresh token, and user info.</returns>
    [HttpPost("login")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
    /// </summary>
    /// <param name="command">The expired access token and the refresh token.</param>
    /// <returns>New access and refresh token pair.</returns>
    [HttpPost("refresh-token")]
    [EnableRateLimiting(RateLimitingServiceExtensions.AuthPolicyName)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>
    /// Logs out the current user by revoking their refresh token.
    /// </summary>
    /// <param name="command">The refresh token to revoke.</param>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] RevokeRefreshTokenCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(null!, "Logged out successfully."));
    }

    /// <summary>
    /// Lists all users with pagination. Admin only.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    [HttpGet("users")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize));
        return Ok(ApiResponse<PaginatedList<UserDto>>.Ok(result));
    }

    /// <summary>
    /// Retrieves a user by their unique identifier. Admin only.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    [HttpGet("users/{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        if (result is null)
            return NotFound(ApiResponse<UserDto>.Fail($"User with Id '{id}' was not found."));

        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    /// <summary>
    /// Updates a user's role, status, or employee link. Admin only.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="dto">Fields to update (role, isActive, employeeId).</param>
    [HttpPut("users/{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var currentUserId = GetCurrentUserId();
        var command = new UpdateUserCommand(id, dto.Role, dto.IsActive, dto.EmployeeId, currentUserId);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    /// <summary>
    /// Soft-deletes a user account. Admin only.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    [HttpDelete("users/{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        await _mediator.Send(new DeleteUserCommand(id, currentUserId));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    /// <summary>
    /// Resets another user's password. Admin only.
    /// </summary>
    /// <param name="id">The target user's unique identifier.</param>
    /// <param name="dto">The new password.</param>
    [HttpPut("users/{id:guid}/reset-password")]
    [Authorize(Roles = Roles.Admin)]
    [EnableRateLimiting(RateLimitingServiceExtensions.PasswordChangePolicyName)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
    {
        var currentUserId = GetCurrentUserId();
        await _mediator.Send(new AdminResetPasswordCommand(id, dto.NewPassword, currentUserId));
        return Ok(ApiResponse<object>.Ok(null!, "Password has been reset successfully."));
    }

    /// <summary>
    /// Changes the current user's own password.
    /// </summary>
    /// <param name="dto">Current password and new password.</param>
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
