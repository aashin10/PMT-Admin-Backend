using MediatR;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Query.Status;

namespace PmtAdmin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("project-statuses")]
        public async Task<IActionResult> GetAllProjectStatuses()
        {
            var query = new GetAllProjectStatusQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}