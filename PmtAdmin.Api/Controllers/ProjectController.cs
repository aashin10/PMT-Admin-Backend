using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context;

namespace PmtAdmin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Key = dto.Key,
                Description = dto.Description,
                CustomerOrgName = dto.CustomerOrgName,
                CustomerDomainUrl = dto.CustomerDomainUrl,
                PocEmail = dto.PocEmail,
                PocPhone = dto.PocPhone,
                ProjectManagerId = dto.ProjectManagerId,
                ProjectManagerRoleId = dto.ProjectManagerRoleId,
                StatusId = dto.StatusId,
                DeliveryUnitId = dto.DeliveryUnitId,
                TemplateId = dto.TemplateId,
                CreatedAt = DateTime.UtcNow,
                IsImportedFromJira = false
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            return Ok(project);
        }
    }
}

