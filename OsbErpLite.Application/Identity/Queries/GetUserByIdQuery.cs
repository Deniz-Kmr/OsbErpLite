using MediatR;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
