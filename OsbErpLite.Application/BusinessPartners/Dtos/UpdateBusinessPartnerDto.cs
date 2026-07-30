using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.BusinessPartners.Dtos;

public class UpdateBusinessPartnerDto
{
    public string Name { get; set; } = default!;
    public string? TaxNumber { get; set; }
    public BusinessPartnerType Type { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}
