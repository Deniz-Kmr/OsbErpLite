using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class Warehouse : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    // navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
