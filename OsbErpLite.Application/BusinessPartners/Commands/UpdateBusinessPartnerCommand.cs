using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public record UpdateBusinessPartnerCommand(
    Guid Id,
    string Name,
    string? TaxNumber,
    BusinessPartnerType Type,
    string? Email,
    string? Phone,
    string? Address,
    string? City
) : IRequest<BusinessPartnerDto>;
