using MediatR;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Commands;

// TODO: ileride rate limiting eklenecek
public record LoginCommand(string Username, string Password) : IRequest<AuthResponseDto>;
