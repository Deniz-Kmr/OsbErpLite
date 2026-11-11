using MediatR;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
