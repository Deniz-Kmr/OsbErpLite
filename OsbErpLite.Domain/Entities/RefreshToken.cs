using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class RefreshToken : AuditableEntity
{
    public string Token { get; set; } = default!;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? CreatedByIp { get; set; }

    // navigation
    public User User { get; set; } = default!;
}
