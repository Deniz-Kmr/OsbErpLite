using AutoMapper;
using OsbErpLite.Application.HumanResources.Employees.Commands;
using OsbErpLite.Application.HumanResources.Employees.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.HumanResources.Employees.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position.Title))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));

        CreateMap<CreateEmployeeCommand, Employee>();

        CreateMap<UpdateEmployeeCommand, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
