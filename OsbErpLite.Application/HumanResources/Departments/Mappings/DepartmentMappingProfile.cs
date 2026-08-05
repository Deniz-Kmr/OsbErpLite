using AutoMapper;
using OsbErpLite.Application.HumanResources.Departments.Commands;
using OsbErpLite.Application.HumanResources.Departments.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.HumanResources.Departments.Mappings;

public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

        CreateMap<CreateDepartmentCommand, Department>();

        CreateMap<UpdateDepartmentCommand, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
