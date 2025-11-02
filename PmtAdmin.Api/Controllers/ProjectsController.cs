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
            [FromQuery] string? statusIds = null,
            [FromQuery] string? deliveryUnitIds = null,
            [FromQuery] string? projectManagerIds = null)
        {
            // Parse comma-separated strings to lists
            List<int>? statusIdList = null;
            if (!string.IsNullOrWhiteSpace(statusIds))
            {
                statusIdList = statusIds.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            }

            List<int>? deliveryUnitIdList = null;
            if (!string.IsNullOrWhiteSpace(deliveryUnitIds))
            {
                deliveryUnitIdList = deliveryUnitIds.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            }

            List<int>? projectManagerIdList = null;
            if (!string.IsNullOrWhiteSpace(projectManagerIds))
            {
                projectManagerIdList = projectManagerIds.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            }

            var query = new GetAllProjectsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                StatusIds = statusIdList,
                DeliveryUnitIds = deliveryUnitIdList,
                ProjectManagerIds = projectManagerIdList
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

        /// <summary>
        /// Get all unique project managers
        /// </summary>
        [HttpGet("managers")]
        public async Task<IActionResult> GetUniqueProjectManagers()
        {
            var query = new GetUniqueProjectManagersQuery();
            var result = await _mediator.Send(query);
            
            if (result.Status == 200)
            {
                return Ok(result);
            }
            return StatusCode(result.Status, result);
        }
    }
}