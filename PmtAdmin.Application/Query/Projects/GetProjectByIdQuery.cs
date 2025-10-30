using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Projects
{
    public class GetProjectByIdQuery : IRequest<ApiResponse<ProjectDTO>>
    {
        public Guid Id { get; set; }
    }
}
