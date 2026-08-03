using MediatR;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Identity.Commands;

public class ChangeOwnPasswordCommandHandler : IRequestHandler<ChangeOwnPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChangeOwnPasswordCommandHandler> _logger;

    public ChangeOwnPasswordCommandHandler(IUserRepository userRepository, ILogger<ChangeOwnPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(ChangeOwnPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with Id '{request.UserId}' was not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mevcut şifre hatalı.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User {Username} (Id: {UserId}) changed their password",
            user.Username, user.Id);
    }
}
