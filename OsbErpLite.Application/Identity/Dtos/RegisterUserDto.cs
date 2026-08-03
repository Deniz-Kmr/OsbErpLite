using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Dtos;

public record RegisterUserDto(
    string Username,
    string Password,
    UserRole Role,
    Guid? EmployeeId);
