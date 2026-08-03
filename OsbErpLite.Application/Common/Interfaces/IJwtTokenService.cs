using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpiry();
}
