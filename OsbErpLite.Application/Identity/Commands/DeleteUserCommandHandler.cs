using MediatR;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Commands;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IUserRepository userRepository, ILogger<DeleteUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with Id '{request.Id}' was not found.");

        if (request.CurrentUserId == request.Id)
            throw new InvalidOperationException("Kendi hesabınızı devre dışı bırakamazsınız.");

        if (user.Role == UserRole.Admin && user.IsActive)
        {
            var activeAdminCount = await _userRepository.GetActiveAdminCountAsync(cancellationToken);
            if (activeAdminCount <= 1)
                throw new InvalidOperationException("Sistemde en az bir aktif Admin bulunmalıdır.");
        }

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Admin {AdminId} deactivated user {Username} (Id: {UserId})",
            request.CurrentUserId, user.Username, user.Id);
    }
}
