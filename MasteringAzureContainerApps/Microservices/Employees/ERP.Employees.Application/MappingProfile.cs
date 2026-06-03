using AutoMapper;
using ERP.Common.Domain;
using ERP.Common.DTO;

namespace ERP.Employees.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, CreateEmployeeDto>().ReverseMap();
        }
    }
}
