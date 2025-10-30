using MediatR;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Query.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var query = new GetAllProjectsQuery();
            var result = await _mediator.Send(query);
            
            if (result.Status == 200)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var query = new GetProjectByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            
            if (result.Status == 200)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var command = new DeleteProjectCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.Status == 200)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }
    }
}