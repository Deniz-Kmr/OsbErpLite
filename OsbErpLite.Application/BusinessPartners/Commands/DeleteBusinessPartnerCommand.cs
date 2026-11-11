using MediatR;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public record DeleteBusinessPartnerCommand(Guid Id) : IRequest<Unit>;
