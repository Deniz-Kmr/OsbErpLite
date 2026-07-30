using AutoMapper;
using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;

namespace OsbErpLite.Application.BusinessPartners.Queries;

public class GetAllBusinessPartnersQueryHandler : IRequestHandler<GetAllBusinessPartnersQuery, PaginatedList<BusinessPartnerDto>>
{
    private readonly IBusinessPartnerRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBusinessPartnersQueryHandler(IBusinessPartnerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<BusinessPartnerDto>> Handle(GetAllBusinessPartnersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<BusinessPartnerDto>>(items);
        return new PaginatedList<BusinessPartnerDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
