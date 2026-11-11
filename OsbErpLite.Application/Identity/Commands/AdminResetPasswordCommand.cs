using MediatR;

namespace OsbErpLite.Application.Identity.Commands;

public record AdminResetPasswordCommand(Guid UserId, string NewPassword, Guid AdminUserId) : IRequest;
