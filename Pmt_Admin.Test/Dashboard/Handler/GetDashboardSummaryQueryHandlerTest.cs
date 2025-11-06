using AutoMapper;
using Moq;
using Pmt_Admin.Test.Dashboard.Mock;
using PmtAdmin.Application.Handlers.Dashboard;
using PmtAdmin.Application.Query.Dashboard;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Dashboard;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Dashboard.Handler
{
    public class GetDashboardSummaryQueryHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDashboardRepository> _dashboardRepositoryMock;
        private readonly GetDashboardSummaryQueryHandler _handler;

        public GetDashboardSummaryQueryHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _dashboardRepositoryMock = new Mock<IDashboardRepository>();
            _handler = new GetDashboardSummaryQueryHandler(
                _mapperMock.Object,
                _dashboardRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_With_Dashboard_Summary()
        {
            // Arrange
            var projects = DashboardMock.GetProjectList();
            var deliveryUnits = DashboardMock.GetDeliveryUnitList();

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((projects, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Dashboard summary retrieved successfully.");
            result.Data.ShouldNotBeNull();

            // Verify totals
            result.Data.TotalProjects.ShouldBe(6);
            result.Data.InProgressProjects.ShouldBe(3); // Active status
            result.Data.OnHoldProjects.ShouldBe(1); // Inactive status
            result.Data.CompletedProjects.ShouldBe(2); // Completed status
            result.Data.TotalDeliveryUnits.ShouldBe(3);

            // Verify project statuses list
            result.Data.ProjectStatuses.ShouldNotBeNull();
            result.Data.ProjectStatuses.Count.ShouldBe(3);

            _dashboardRepositoryMock.Verify(repo => repo.GetDashboardDataAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Calculate_Correct_Project_Counts_By_Status()
        {
            // Arrange
            var projects = DashboardMock.GetProjectList();
            var deliveryUnits = DashboardMock.GetDeliveryUnitList();

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((projects, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var activeCount = projects.Count(p => p.Status?.Name == "Active");
            var inactiveCount = projects.Count(p => p.Status?.Name == "Inactive");
            var completedCount = projects.Count(p => p.Status?.Name == "Completed");

            result.Data.InProgressProjects.ShouldBe(activeCount);
            result.Data.OnHoldProjects.ShouldBe(inactiveCount);
            result.Data.CompletedProjects.ShouldBe(completedCount);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_No_Projects_Found()
        {
            // Arrange
            var emptyProjects = DashboardMock.EmptyProjectList();
            var deliveryUnits = DashboardMock.GetDeliveryUnitList();

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((emptyProjects, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(404); // Not Found status
            result.Message.ShouldBe("No active projects found.");
            result.Data.ShouldBeNull();

            _dashboardRepositoryMock.Verify(repo => repo.GetDashboardDataAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Projects_Is_Null()
        {
            // Arrange
            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((null, DashboardMock.GetDeliveryUnitList()));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(404); // Not Found status
            result.Message.ShouldBe("No active projects found.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task Handle_Should_Handle_Projects_Without_Status()
        {
            // Arrange
            var projectsWithoutStatus = DashboardMock.GetProjectsWithoutStatus();
            var deliveryUnits = DashboardMock.GetDeliveryUnitList();

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((projectsWithoutStatus, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Data.TotalProjects.ShouldBe(2);
            result.Data.InProgressProjects.ShouldBe(0); // No status means not counted
            result.Data.OnHoldProjects.ShouldBe(0);
            result.Data.CompletedProjects.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_Should_Create_ProjectStatus_For_Each_Delivery_Unit()
        {
            // Arrange
            var projects = DashboardMock.GetProjectList();
            var deliveryUnits = DashboardMock.GetDeliveryUnitList();

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((projects, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ProjectStatuses.Count.ShouldBe(deliveryUnits.Count);

            foreach (var du in deliveryUnits)
            {
                var projectStatus = result.Data.ProjectStatuses
                    .FirstOrDefault(ps => ps.DeliveryUnit == du.Name);

                projectStatus.ShouldNotBeNull();
                projectStatus.DeliveryUnit.ShouldBe(du.Name);
            }
        }

        [Fact]
        public async Task Handle_Should_Only_Count_Projects_With_Matching_Status_Names()
        {
            // Arrange
            var activeStatus = new ProjectStatus { Id = 1, Name = "Active" };
            var customStatus = new ProjectStatus { Id = 4, Name = "Pending" }; // Different status

            var projects = new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    DeliveryUnitId = 1,
                    Status = activeStatus
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    DeliveryUnitId = 1,
                    Status = customStatus // This should not be counted in any category
                }
            };

            var deliveryUnits = new List<DeliveryUnit>
            {
                new DeliveryUnit { Id = 1, Name = "Team" }
            };

            _dashboardRepositoryMock
                .Setup(repo => repo.GetDashboardDataAsync())
                .ReturnsAsync((projects, deliveryUnits));

            var query = new GetDashboardSummaryQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.TotalProjects.ShouldBe(2);
            result.Data.InProgressProjects.ShouldBe(1); // Only "Active"
            result.Data.OnHoldProjects.ShouldBe(0);
            result.Data.CompletedProjects.ShouldBe(0);
        }
    }
}
