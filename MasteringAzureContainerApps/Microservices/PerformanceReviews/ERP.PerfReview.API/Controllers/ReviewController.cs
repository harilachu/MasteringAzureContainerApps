using ERP.Common.Core;
using ERP.PerfReview.Application.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;
using Synaptrix;

namespace ERP.PerfReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ISender sender;

        public ReviewController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet("{empId}/projects")]
        public IActionResult Get(string empId, CancellationToken cancellationToken)
        {
            var query = new GetProjectHistoryQuery(empId);
            ValueTask<Result<GetProjectHistoryResult>> queryResult = sender.Send(query, cancellationToken);

            var result = queryResult.Result;

            return result.Match<GetProjectHistoryResult, IActionResult>(
               projectHistoryResult => Ok(projectHistoryResult.Projects),
               error => BadRequest(new
               {
                   status = error.Status,
                   message = error.Message
               }));
        }
    }
}
