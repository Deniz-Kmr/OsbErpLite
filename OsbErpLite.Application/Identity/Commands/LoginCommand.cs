using MediatR;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Commands;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponseDto>;
