using MediatR;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
