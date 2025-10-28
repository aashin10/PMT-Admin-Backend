using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class DeleteUsersCommandHandler : IRequestHandler<DeleteUsersCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUsersCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<string>> Handle(DeleteUsersCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.DeleteUsersByIdsAsync(request.Ids);
            return ApiResponse<string>.Success("Users deleted successfully");
        }
    }
}