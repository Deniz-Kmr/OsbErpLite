using MediatR;

namespace OsbErpLite.Application.Identity.Commands;

public record RevokeRefreshTokenCommand(string RefreshToken) : IRequest<Unit>;
