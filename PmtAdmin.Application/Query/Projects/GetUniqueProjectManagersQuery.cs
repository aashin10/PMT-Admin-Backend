using MediatR;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Projects
{
    public class GetUniqueProjectManagersQuery : IRequest<ApiResponse<IReadOnlyList<ProjectManagerInfo>>>
    {
    }
}