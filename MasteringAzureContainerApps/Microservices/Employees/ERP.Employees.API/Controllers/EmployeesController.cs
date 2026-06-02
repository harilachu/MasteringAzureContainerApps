using ERP.Common.Core;
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
        public IActionResult Get([FromRoute] string id, [FromQuery] string department, CancellationToken cancellationToken)
        {
            var query = new GetEmployeeQuery(id, department);
            ValueTask<Common.Core.Result<GetEmployeeResult>> queryResult = sender.Send(query, cancellationToken);

            var result = queryResult.Result;

            return result.Match<GetEmployeeResult, IActionResult>(
               employeeResult => Ok(employeeResult.Employee),
               error => BadRequest(new
               {
                   status = error.Status,
                   message = error.Message
               }));
        }

        [HttpGet()]
        public IActionResult GetEmployeeByEmpId([FromQuery] string empId, [FromQuery] string department, CancellationToken cancellationToken)
        {
            var query = new GetEmployeeByEmpIdQuery(empId, department);
            ValueTask<Common.Core.Result<GetEmployeeByEmpIdResult>> queryResult = sender.Send(query, cancellationToken);

            var result = queryResult.Result;

            return result.Match<GetEmployeeByEmpIdResult, IActionResult>(
                employeeResult => Ok(employeeResult.Employee),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }
    }
}

