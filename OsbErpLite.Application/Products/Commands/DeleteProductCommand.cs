using MediatR;

namespace OsbErpLite.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
