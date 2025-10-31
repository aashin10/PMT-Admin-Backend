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

        /// <summary>
        /// Get all projects for table view with pagination and multi-select filtering
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, options: 10, 25, 50)</param>
        /// <param name="searchTerm">Search by project name, key, or manager name</param>
        /// <param name="statusIds">Filter by multiple project status IDs (comma-separated)</param>
        /// <param name="deliveryUnitIds">Filter by multiple delivery unit IDs (comma-separated)</param>
        /// <param name="projectManagerIds">Filter by multiple project manager IDs (comma-separated)</param>
        [HttpGet]
        public async Task<IActionResult> GetAllProjects(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] List<int>? statusIds = null,
            [FromQuery] List<int>? deliveryUnitIds = null,
            [FromQuery] List<int>? projectManagerIds = null)
        {
            var query = new GetAllProjectsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                StatusIds = statusIds,
                DeliveryUnitIds = deliveryUnitIds,
                ProjectManagerIds = projectManagerIds
            };

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch between route and body");

            var result = await _mediator.Send(command);
            
            if (result.Status == 404)
                return NotFound(result.Message);
            
            if (result.Status != 200)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}