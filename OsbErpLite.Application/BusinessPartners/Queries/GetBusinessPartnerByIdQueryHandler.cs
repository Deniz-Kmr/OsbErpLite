using AutoMapper;
using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.BusinessPartners.Queries;

public class GetBusinessPartnerByIdQueryHandler : IRequestHandler<GetBusinessPartnerByIdQuery, BusinessPartnerDto?>
{
    private readonly IBusinessPartnerRepository _repository;
    private readonly IMapper _mapper;

    public GetBusinessPartnerByIdQueryHandler(IBusinessPartnerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessPartnerDto?> Handle(GetBusinessPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<BusinessPartnerDto>(entity);
    }
}
