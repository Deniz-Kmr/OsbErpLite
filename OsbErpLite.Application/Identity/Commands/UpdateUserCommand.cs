using MediatR;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Commands;

public record UpdateUserCommand(
    Guid Id,
    UserRole Role,
    bool IsActive,
    Guid? EmployeeId,
    Guid CurrentUserId) : IRequest<UserDto>;
