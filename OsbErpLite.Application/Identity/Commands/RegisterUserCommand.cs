using MediatR;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Commands;

public record RegisterUserCommand(
    string Username,
    string Password,
    UserRole Role,
    Guid? EmployeeId) : IRequest<AuthResponseDto>;
