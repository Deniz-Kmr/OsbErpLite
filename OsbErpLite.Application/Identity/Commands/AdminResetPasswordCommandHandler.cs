using MediatR;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Identity.Commands;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AdminResetPasswordCommandHandler> _logger;

    public AdminResetPasswordCommandHandler(IUserRepository userRepository, ILogger<AdminResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var targetUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with Id '{request.UserId}' was not found.");

        targetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(targetUser, cancellationToken);

        _logger.LogInformation("Admin {AdminId} reset password for user {Username} (Id: {UserId})",
            request.AdminUserId, targetUser.Username, targetUser.Id);
    }
}
