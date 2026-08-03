using MediatR;

namespace OsbErpLite.Application.Identity.Commands;

// TODO: ileride rate limiting eklenecek
public record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;
