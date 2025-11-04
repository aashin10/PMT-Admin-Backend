using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("filter")]
        public async Task<ApiResponse<List<UserDto>>> GetAllUsers([FromBody] GetAllUsersQuery query)
        {
            var users = await _mediator.Send(query);
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<UserDto>> GetUserById(int id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return user;
        }

        [HttpPost]
        public async Task<ApiResponse<List<UserDto>>> CreateUser([FromBody] CreateUserCommand command)
        {
            var newUsers = await _mediator.Send(command);
            return newUsers;
        }

        [HttpDelete]
        public async Task<ApiResponse<string>> DeleteUsers([FromBody] DeleteUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPost("bulk-import")]
        public async Task<ApiResponse<BulkImportResultDto>> BulkImportUsers([FromBody] BulkImportUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPost("paginated")]
        public async Task<ApiResponse<PaginatedUserResponseDto>> GetUsersWithPagination([FromBody] GetUsersWithPaginationQuery query)
        {
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<UserDto>> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPost("jira-import")]
        public async Task<ApiResponse<JiraCsvImportResultDto>> JiraUsersImportCsv([FromBody] JiraCsvImportUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
