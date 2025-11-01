using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto.DashboardDTO;
using PmtAdmin.Application.Query.Dashboard;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Dashboard
{
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, ApiResponse<DashboardSummaryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IDashboardRepository _dashboardRepository;

        public GetDashboardSummaryQueryHandler(IMapper mapper, IDashboardRepository dashboardRepository)
        {
            _mapper = mapper;
            _dashboardRepository = dashboardRepository;
        }

        public async Task<ApiResponse<DashboardSummaryDto>> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var (projects, deliveryUnits) = await _dashboardRepository.GetDashboardDataAsync();

            if (projects == null || !projects.Any())
            {
                return ApiResponse<DashboardSummaryDto>.Fail("No projects found.");
            }

            var totalProjects = projects.Count;
            var inProgressCount = projects.Count(p => p.Status != null && p.Status.Name == "Active");
            var onHoldCount = projects.Count(p => p.Status != null && p.Status.Name == "Inactive");
            var completedCount = projects.Count(p => p.Status != null && p.Status.Name == "Completed");
            var totalDeliveryUnits = deliveryUnits.Count;

            var projectStatusList = deliveryUnits.Select(du => new ProjectStatusDto
            {
                DeliveryUnit = du.Name,
                InProgress = projects.Count(p => p.DeliveryUnitId == du.Id && p.Status != null && p.Status.Name == "Active"),
                OnHold = projects.Count(p => p.DeliveryUnitId == du.Id && p.Status != null && p.Status.Name == "Inactive"),
                Completed = projects.Count(p => p.DeliveryUnitId == du.Id && p.Status != null && p.Status.Name == "Completed"),
                Total = projects.Count(p => p.DeliveryUnitId == du.Id)
            }).ToList();

            var dashboardDto = new DashboardSummaryDto
            {
                TotalProjects = totalProjects,
                InProgressProjects = inProgressCount,
                OnHoldProjects = onHoldCount,
                CompletedProjects = completedCount,
                TotalDeliveryUnits = totalDeliveryUnits,
                ProjectStatuses = projectStatusList
            };

            return ApiResponse<DashboardSummaryDto>.Success(dashboardDto, "Dashboard summary retrieved successfully.");
        }
    }
}
