using MediatR;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command.Projects
{
    public class DeleteProjectCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
