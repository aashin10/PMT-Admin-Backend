using MediatR;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Api.Controllers
{
    [ApiController]
    [Route("api/delivery-unit")]
    [Produces("application/json")]
    public class DeliveryUnitController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryUnitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all delivery units
        /// </summary>
        /// <returns>List of delivery units</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DuDto>>> GetAll()
        {
            var query = new PmtAdmin.Application.Query.Delivery_Unit.GetAllDuQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //[HttpPost("filter")]
        //public async Task<ApiResponse<List<UserDto>>> AddnewDu([FromBody] CreateDuCommand command)
        //{
        //    var users = await _mediator.Send(command);
        //    return ;
        //}
        [HttpPost]
        public async Task<ApiResponse<DuDto>> CreateDu([FromBody] CreateDuCommand command)
        {
            var newDu = await _mediator.Send(command);
            Console.WriteLine($"➡️ Received CreateDU: Name={command.Name}, Code={command.Code}, Desc={command.Description}, HeadName={command.HeadName}, HeadEmail={command.HeadEmail}");

            return newDu;
        }
        /// <summary>
        /// Create a new delivery unit
        /// </summary>
        /// <param name="command">Delivery unit creation details</param>
        /// <returns>Created delivery unit</returns>
        [HttpPut("{id}")]  // ← FIXED: Added {id} parameter
       
        public async Task<ActionResult<ApiResponse<DuDto>>> UpdateDu(int id, [FromBody] UpdateDuCommand command)
        {
            // Ensure route ID matches body ID
            command.Id = id;

            if (id <= 0)
                return BadRequest(ApiResponse<DuDto>.Fail("Invalid delivery unit ID."));

            var updatedDu = await _mediator.Send(command);

            if (updatedDu == null || updatedDu.Data == null)
                return NotFound(ApiResponse<DuDto>.Fail("Delivery Unit not found."));

            return Ok(updatedDu);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDu(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteDuCommand(id);
            var result = await _mediator.Send(command, cancellationToken);


            return Ok(result);
        }



    }
}