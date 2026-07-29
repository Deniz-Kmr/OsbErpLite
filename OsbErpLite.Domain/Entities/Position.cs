using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class Position : AuditableEntity
{
    public string Title { get; set; } = default!;
    public int Level { get; set; }

    // navigation
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
