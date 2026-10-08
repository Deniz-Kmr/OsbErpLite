using MediatR;

namespace OsbErpLite.Application.Identity.Commands;

public record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
