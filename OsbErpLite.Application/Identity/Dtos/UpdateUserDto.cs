using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Dtos;

public record UpdateUserDto(UserRole Role, bool IsActive, Guid? EmployeeId);
