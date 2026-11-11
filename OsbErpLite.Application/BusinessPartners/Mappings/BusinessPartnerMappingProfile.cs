using AutoMapper;
using OsbErpLite.Application.BusinessPartners.Commands;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.BusinessPartners.Mappings;

public class BusinessPartnerMappingProfile : Profile
{
    public BusinessPartnerMappingProfile()
    {
        CreateMap<BusinessPartner, BusinessPartnerDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<CreateBusinessPartnerCommand, BusinessPartner>();
        CreateMap<UpdateBusinessPartnerCommand, BusinessPartner>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
