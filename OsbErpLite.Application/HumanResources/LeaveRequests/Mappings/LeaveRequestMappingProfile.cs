using AutoMapper;
using OsbErpLite.Application.HumanResources.LeaveRequests.Commands;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Mappings;

public class LeaveRequestMappingProfile : Profile
{
    public LeaveRequestMappingProfile()
    {
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.ApprovedByEmployeeName, opt => opt.MapFrom(src => src.ApprovedByEmployee != null ? src.ApprovedByEmployee.FullName : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateLeaveRequestCommand, LeaveRequest>();
    }
}
