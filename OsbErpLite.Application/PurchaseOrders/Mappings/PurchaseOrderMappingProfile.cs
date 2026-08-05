using AutoMapper;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.PurchaseOrders.Mappings;

public class PurchaseOrderMappingProfile : Profile
{
    public PurchaseOrderMappingProfile()
    {
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
            .ForMember(dest => dest.ApprovedByEmployeeName,
                opt => opt.MapFrom(src => src.ApprovedByEmployee != null
                    ? src.ApprovedByEmployee.FullName
                    : null));

        CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU));
    }
}
