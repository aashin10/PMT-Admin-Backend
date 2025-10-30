using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Query.Permissions;

namespace PmtAdmin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class PermissionsController : ControllerBase
        {
            private readonly IMediator _mediator;

            public PermissionsController(IMediator mediator)
            {
                _mediator = mediator;
            }

            // GET: api/permissions
            [HttpGet]
            public async Task<IActionResult> GetAllPermissions()
            {
                var query = new GetAllPermissionsQuery();
                var permissions = await _mediator.Send(query);

                return Ok(new { data = permissions, count = permissions?.Count ?? 0 });
            }
        }
    }

