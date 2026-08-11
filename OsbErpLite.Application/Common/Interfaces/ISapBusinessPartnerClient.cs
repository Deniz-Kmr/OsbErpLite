namespace OsbErpLite.Application.Common.Interfaces;

public class SapBusinessPartnerRecord
{
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public string? SapId { get; set; }
}

public interface ISapBusinessPartnerClient
{
    Task<List<SapBusinessPartnerRecord>> GetBusinessPartnersAsync(CancellationToken cancellationToken = default);
}
