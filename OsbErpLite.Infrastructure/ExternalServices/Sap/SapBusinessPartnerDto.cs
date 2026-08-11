using System.Text.Json.Serialization;

namespace OsbErpLite.Infrastructure.ExternalServices.Sap;

/// <summary>
/// SAP API_BUSINESS_PARTNER servisinden donen ham veri modeli
/// </summary>
public class SapBusinessPartnerDto
{
    [JsonPropertyName("BusinessPartner")]
    public string BusinessPartner { get; set; } = default!;

    [JsonPropertyName("BusinessPartnerFullName")]
    public string? BusinessPartnerFullName { get; set; }

    [JsonPropertyName("BusinessPartnerName")]
    public string? BusinessPartnerName { get; set; }

    [JsonPropertyName("BusinessPartnerCategory")]
    public string? BusinessPartnerCategory { get; set; }

    [JsonPropertyName("TaxNumber1")]
    public string? TaxNumber1 { get; set; }

    [JsonPropertyName("TaxNumber2")]
    public string? TaxNumber2 { get; set; }

    [JsonPropertyName("SearchTerm1")]
    public string? SearchTerm1 { get; set; }

    [JsonPropertyName("BusinessPartnerGrouping")]
    public string? BusinessPartnerGrouping { get; set; }
}
