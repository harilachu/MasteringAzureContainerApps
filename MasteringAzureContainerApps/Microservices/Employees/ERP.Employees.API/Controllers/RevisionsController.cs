using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Employees.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RevisionsController : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get(CancellationToken cancellationToken)
        {
            return Ok(new
            {
                CurrentRevision = "Revision A"
            });
        }
    }
}
