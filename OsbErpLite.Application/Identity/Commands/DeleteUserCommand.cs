using MediatR;

namespace OsbErpLite.Application.Identity.Commands;

public record DeleteUserCommand(Guid Id, Guid CurrentUserId) : IRequest;
