using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Query;
using System.Threading.Tasks;

namespace PmtAdmin.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/roles?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetRoles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetRolesQuery { Page = page, PageSize = pageSize };
            var roles = await _mediator.Send(query);
            return Ok(new { data = roles, page, pageSize });
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var query = new GetRoleByIdQuery { Id = id };
            var role = await _mediator.Send(query);

            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roleId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRole), new { id = roleId }, new { id = roleId });
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            command.Id = id;
            var result = await _mediator.Send(command);

            if (result == null || result.Status == 404)
                return NotFound(new { message = "Role not found" });

            if (result.Status >= 400)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Role updated successfully", data = result.Data });
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var command = new DeleteRoleCommand { Id = id };
            var result = await _mediator.Send(command);

            if (result == null || result.Status == 404)
                return NotFound(new { message = "Role not found" });

            if (result.Status >= 400)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Role deleted successfully" });
        }
    }
}