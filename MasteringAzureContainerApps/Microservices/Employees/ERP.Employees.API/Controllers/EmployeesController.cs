using ERP.Common.DTO;
using ERP.Employees.Application.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;
using Synaptrix;

namespace ERP.Employees.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ISender sender;

        public EmployeesController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet("{id}")]
        public EmployeeDto Get(int id, CancellationToken cancellationToken)
        {
            var query = new GetEmployeeQuery(id);
            var result = sender.Send(query, cancellationToken);
            return result.Result.Value.Employee;
        }
    }

}

