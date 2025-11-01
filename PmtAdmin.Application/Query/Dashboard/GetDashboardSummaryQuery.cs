using MediatR;
using PmtAdmin.Application.Dto.DashboardDTO;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Dashboard
{
    public class GetDashboardSummaryQuery : IRequest<ApiResponse<DashboardSummaryDto>>
    {
    }
}
