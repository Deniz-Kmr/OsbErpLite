using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public record SyncBusinessPartnersFromSapCommand : IRequest<SyncResultDto>;
