using MediatR;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Query.Dashboard;

namespace PmtAdmin.Api.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var response = await _mediator.Send(new GetDashboardSummaryQuery());
            return Ok(response);
        }

        [HttpGet("activity")]
        public async Task<IActionResult> GetActivity()
        {
            var response = await _mediator.Send(new GetProjectActivityQuery());
            return Ok(response);
        }
    }
}
