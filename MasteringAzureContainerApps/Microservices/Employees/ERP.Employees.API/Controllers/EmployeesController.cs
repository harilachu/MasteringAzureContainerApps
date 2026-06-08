using AutoMapper.Configuration.Annotations;
using ERP.Common.Core;
using ERP.Common.DTO;
using ERP.Employees.Application.CQRS.Commands;
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
        private readonly ILogger<EmployeesController> logger;

        public EmployeesController(ISender sender, ILogger<EmployeesController> logger)
        {
            this.sender = sender;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] string id, [FromQuery] string department, CancellationToken cancellationToken)
        {
            var query = new GetEmployeeQuery(id, department);
            ValueTask<Result<GetEmployeeResult>> queryResult = sender.Send(query, cancellationToken);

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
            var allHeaders = this.HttpContext.Request.Headers
                .Select(h => $"{h.Key}:{string.Join(';', h.Value)}");
            logger.LogInformation("Incoming request headers: {Headers}", string.Join(", ", allHeaders));

            if (this.HttpContext.Request.Headers.ContainsKey("X-Request-ID"))
            {
                var daprRequestId = this.HttpContext.Request.Headers["X-Request-ID"].FirstOrDefault();
                logger.LogInformation("Request ID from Dapr: {DaprRequestId}", daprRequestId);
            }

            var query = new GetEmployeeByEmpIdQuery(empId, department);
            ValueTask<Result<GetEmployeeByEmpIdResult>> queryResult = sender.Send(query, cancellationToken);

            var result = queryResult.Result;

            return result.Match<GetEmployeeByEmpIdResult, IActionResult>(
                employeeResult => Ok(employeeResult.Employee),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }

        [HttpGet("All")]
        public IActionResult GetAllEmployeesByDept([FromQuery] string department, CancellationToken cancellationToken)
        {
            var query = new GetAllEmployeesByDeptQuery(department);
            ValueTask<Result<GetAllEmployeesByDeptResult>> queryResult = sender.Send(query, cancellationToken);

            var result = queryResult.Result;

            return result.Match<GetAllEmployeesByDeptResult, IActionResult>(
                employeesResult => Ok(employeesResult.Employee),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }

        [HttpPost()]
        public IActionResult Post([FromBody] CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                return BadRequest(ModelState);
            }

            var command = new AddEmployeeCommand(employeeDto);
            var commandResult = sender.Send(command);
            var result = commandResult.Result;

            return result.Match<AddEmployeeResult, IActionResult>(
                addedResult => Ok(addedResult),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }

        [HttpDelete]
        public IActionResult DeleteEmployeeByEmpId([FromQuery] string empId, [FromQuery] string department, CancellationToken cancellationToken)
        {
            var command = new DeleteEmployeeCommand(empId, department);
            var commandResult = sender.Send(command);
            var result = commandResult.Result;

            return result.Match<DeleteEmployeeResult, IActionResult>(
                deletedResult => Ok(deletedResult),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }

        [HttpPut]
        public IActionResult UpdateEmployee([FromBody] CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                return BadRequest(ModelState);
            }
            var command = new UpdateEmployeeCommand(employeeDto);
            var commandResult = sender.Send(command);
            var result = commandResult.Result;
            return result.Match<UpdateEmployeeResult, IActionResult>(
                updatedResult => Ok(updatedResult),
                error => BadRequest(new
                {
                    status = error.Status,
                    message = error.Message
                }));
        }
    }
}

