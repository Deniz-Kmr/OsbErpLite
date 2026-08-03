using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<UserDto>>(items);
        return new PaginatedList<UserDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
