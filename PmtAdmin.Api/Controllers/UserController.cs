using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Application.Query.Users;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Api.Controllers
{
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

        [HttpGet("paginated")]
        public async Task<ApiResponse<PaginatedResponse<UserDto>>> GetUsersWithPagination(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "name",
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] string? type = null,
            [FromQuery] string? status = null)
        {
            var query = new GetUsersWithPaginationQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Type = type,
                Status = status
            };

            var result = await _mediator.Send(query);
            return result;
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

        [HttpPost("import-csv")]
        public async Task<ApiResponse<List<UserDto>>> ImportUsersFromCsv([FromBody] ImportUsersFromCsvCommand command)
        {
            var importedUsers = await _mediator.Send(command);
            return importedUsers;
        }

        [HttpDelete]
        public async Task<ApiResponse<string>> DeleteUsers([FromBody] DeleteUsersCommand command)
        {
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
