using PmtAdmin.Application.Dto.DashboardDTO;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Dashboard.Mock
{
    public class DashboardMock
    {
        // === Projects Mock Data ===
        public static List<Project> GetProjectList()
        {
            var activeStatus = new ProjectStatus { Id = 1, Name = "Active" };
            var inactiveStatus = new ProjectStatus { Id = 2, Name = "Inactive" };
            var completedStatus = new ProjectStatus { Id = 3, Name = "Completed" };

            return new List<Project>
            {
                new Project
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Name = "Project Alpha",
                    DeliveryUnitId = 1,
                    StatusId = 1,
                    Status = activeStatus,
                    CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Name = "Project Beta",
                    DeliveryUnitId = 1,
                    StatusId = 1,
                    Status = activeStatus,
                    CreatedAt = new DateTime(2024, 2, 20, 14, 45, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    Name = "Project Gamma",
                    DeliveryUnitId = 2,
                    StatusId = 2,
                    Status = inactiveStatus,
                    CreatedAt = new DateTime(2024, 3, 10, 9, 15, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    Name = "Project Delta",
                    DeliveryUnitId = 2,
                    StatusId = 3,
                    Status = completedStatus,
                    CreatedAt = new DateTime(2024, 6, 5, 16, 20, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    Name = "Project Epsilon",
                    DeliveryUnitId = 3,
                    StatusId = 1,
                    Status = activeStatus,
                    CreatedAt = new DateTime(2024, 7, 12, 11, 0, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    Name = "Project Zeta",
                    DeliveryUnitId = 3,
                    StatusId = 3,
                    Status = completedStatus,
                    CreatedAt = new DateTime(2024, 9, 25, 13, 30, 0, DateTimeKind.Utc)
                }
            };
        }

        // === Delivery Units Mock Data ===
        public static List<DeliveryUnit> GetDeliveryUnitList()
        {
            return new List<DeliveryUnit>
            {
                new DeliveryUnit
                {
                    Id = 1,
                    Name = "Development Team"
                },
                new DeliveryUnit
                {
                    Id = 2,
                    Name = "QA Team"
                },
                new DeliveryUnit
                {
                    Id = 3,
                    Name = "DevOps Team"
                }
            };
        }

        // === Project Projections for Chart Data ===
        public static List<ProjectProjection> GetProjectProjectionList()
        {
            return new List<ProjectProjection>
            {
                new ProjectProjection
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    CreatedAt = new DateTime(2024, 1, 12, 14, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    CreatedAt = new DateTime(2024, 2, 8, 9, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    CreatedAt = new DateTime(2024, 3, 15, 16, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    CreatedAt = new DateTime(2024, 6, 20, 11, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    CreatedAt = new DateTime(2024, 9, 10, 13, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("77777777-7777-7777-7777-777777777777"),
                    CreatedAt = new DateTime(2023, 5, 15, 10, 0, 0, DateTimeKind.Utc)
                },
                new ProjectProjection
                {
                    Id = new Guid("88888888-8888-8888-8888-888888888888"),
                    CreatedAt = new DateTime(2023, 8, 22, 15, 0, 0, DateTimeKind.Utc)
                }
            };
        }

        // === Current Month Week Distribution (for Monthly chart) ===
        public static List<ProjectProjection> GetCurrentMonthProjections(int year, int month)
        {
            return new List<ProjectProjection>
            {
                new ProjectProjection
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = new DateTime(year, month, 3, 10, 0, 0, DateTimeKind.Utc) // Week 1
                },
                new ProjectProjection
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    CreatedAt = new DateTime(year, month, 5, 14, 0, 0, DateTimeKind.Utc) // Week 1
                },
                new ProjectProjection
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    CreatedAt = new DateTime(year, month, 10, 9, 0, 0, DateTimeKind.Utc) // Week 2
                },
                new ProjectProjection
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    CreatedAt = new DateTime(year, month, 17, 16, 0, 0, DateTimeKind.Utc) // Week 3
                },
                new ProjectProjection
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    CreatedAt = new DateTime(year, month, 18, 11, 0, 0, DateTimeKind.Utc) // Week 3
                },
                new ProjectProjection
                {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    CreatedAt = new DateTime(year, month, 25, 13, 0, 0, DateTimeKind.Utc) // Week 4
                }
            };
        }

        // === Expected Dashboard Summary DTO ===
        public static DashboardSummaryDto GetExpectedDashboardSummaryDto()
        {
            return new DashboardSummaryDto
            {
                TotalProjects = 6,
                InProgressProjects = 3,
                OnHoldProjects = 1,
                CompletedProjects = 2,
                TotalDeliveryUnits = 3,
                ProjectStatuses = new List<ProjectStatusDto>
                {
                    new ProjectStatusDto
                    {
                        DeliveryUnit = "Development Team",
                        InProgress = 2,
                        OnHold = 0,
                        Completed = 0,
                        Total = 2
                    },
                    new ProjectStatusDto
                    {
                        DeliveryUnit = "QA Team",
                        InProgress = 0,
                        OnHold = 1,
                        Completed = 1,
                        Total = 2
                    },
                    new ProjectStatusDto
                    {
                        DeliveryUnit = "DevOps Team",
                        InProgress = 1,
                        OnHold = 0,
                        Completed = 1,
                        Total = 2
                    }
                }
            };
        }

        // === Empty Lists ===
        public static List<Project> EmptyProjectList() => new List<Project>();
        public static List<DeliveryUnit> EmptyDeliveryUnitList() => new List<DeliveryUnit>();
        public static List<ProjectProjection> EmptyProjectProjectionList() => new List<ProjectProjection>();

        // === Single Project ===
        public static Project GetSingleActiveProject()
        {
            return new Project
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Name = "Single Project",
                DeliveryUnitId = 1,
                StatusId = 1,
                Status = new ProjectStatus { Id = 1, Name = "Active" },
                CreatedAt = DateTime.UtcNow
            };
        }

        // === Projects without Status ===
        public static List<Project> GetProjectsWithoutStatus()
        {
            return new List<Project>
            {
                new Project
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Name = "Project Without Status",
                    DeliveryUnitId = 1,
                    StatusId = null,
                    Status = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Project
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    Name = "Another Project",
                    DeliveryUnitId = 1,
                    StatusId = null,
                    Status = null,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        // === Projects for Quarterly Testing (Current Year) ===
        public static List<ProjectProjection> GetQuarterlyProjections(int year)
        {
            return new List<ProjectProjection>
            {
                new ProjectProjection { Id = new Guid("11111111-1111-1111-1111-111111111111"), CreatedAt = new DateTime(year, 1, 15, 0, 0, 0, DateTimeKind.Utc) }, // Q1
                new ProjectProjection { Id = new Guid("22222222-2222-2222-2222-222222222222"), CreatedAt = new DateTime(year, 2, 20, 0, 0, 0, DateTimeKind.Utc) }, // Q1
                new ProjectProjection { Id = new Guid("33333333-3333-3333-3333-333333333333"), CreatedAt = new DateTime(year, 4, 10, 0, 0, 0, DateTimeKind.Utc) }, // Q2
                new ProjectProjection { Id = new Guid("44444444-4444-4444-4444-444444444444"), CreatedAt = new DateTime(year, 7, 5, 0, 0, 0, DateTimeKind.Utc) },  // Q3
                new ProjectProjection { Id = new Guid("55555555-5555-5555-5555-555555555555"), CreatedAt = new DateTime(year, 7, 25, 0, 0, 0, DateTimeKind.Utc) }, // Q3
                new ProjectProjection { Id = new Guid("66666666-6666-6666-6666-666666666666"), CreatedAt = new DateTime(year, 10, 12, 0, 0, 0, DateTimeKind.Utc) } // Q4
            };
        }

        // === Projects spanning multiple years ===
        public static List<ProjectProjection> GetMultiYearProjections()
        {
            return new List<ProjectProjection>
            {
                new ProjectProjection { Id = new Guid("11111111-1111-1111-1111-111111111111"), CreatedAt = new DateTime(2020, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("22222222-2222-2222-2222-222222222222"), CreatedAt = new DateTime(2021, 6, 20, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("33333333-3333-3333-3333-333333333333"), CreatedAt = new DateTime(2021, 9, 10, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("44444444-4444-4444-4444-444444444444"), CreatedAt = new DateTime(2022, 1, 5, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("55555555-5555-5555-5555-555555555555"), CreatedAt = new DateTime(2023, 4, 12, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("66666666-6666-6666-6666-666666666666"), CreatedAt = new DateTime(2023, 11, 8, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectProjection { Id = new Guid("77777777-7777-7777-7777-777777777777"), CreatedAt = new DateTime(2024, 2, 14, 0, 0, 0, DateTimeKind.Utc) }
            };
        }
    }
}
