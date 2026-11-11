using AutoMapper;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.SalesOrders.Mappings;

public class SalesOrderMappingProfile : Profile
{
    public SalesOrderMappingProfile()
    {
        CreateMap<SalesOrder, SalesOrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.ApprovedByEmployeeName,
                opt => opt.MapFrom(src => src.ApprovedByEmployee != null
                    ? src.ApprovedByEmployee.FullName
                    : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<SalesOrderLine, SalesOrderLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU));
    }
}
