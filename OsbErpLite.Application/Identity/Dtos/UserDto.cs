using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
