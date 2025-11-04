using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command.CustomFields;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomFieldsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomFieldsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<CustomFieldDTO>>> CreateCustomField([FromBody] CreateCustomFieldCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCustomField(Guid id)
        {
            var command = new DeleteCustomFieldCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
