using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class Department : AuditableEntity
{
    public string Name { get; set; } = default!;
    public Guid? ManagerId { get; set; }

    // navigation
    public Employee? Manager { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
