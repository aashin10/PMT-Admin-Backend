using AutoMapper;
using Moq;
using Pmt_Admin.Test.Dashboard.Mock;
using PmtAdmin.Application.Handlers.Dashboard;
using PmtAdmin.Application.Query.Dashboard;
using PmtAdmin.Domain.Persistance.Dashboard;
using Shouldly;

namespace Pmt_Admin.Test.Dashboard.Handler
{
    public class GetProjectActivityQueryHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IProjectReadRepository> _projectReadRepositoryMock;
        private readonly DateTime _testDate = new DateTime(2024, 10, 15, 10, 0, 0, DateTimeKind.Utc);
        private readonly GetProjectActivityQueryHandler _handler;

        public GetProjectActivityQueryHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _projectReadRepositoryMock = new Mock<IProjectReadRepository>();
            _handler = new GetProjectActivityQueryHandler(
                _mapperMock.Object,
                _projectReadRepositoryMock.Object,
                _testDate);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_With_Chart_Data()
        {
            // Arrange
            var projections = DashboardMock.GetProjectProjectionList();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Project activity chart data fetched successfully.");
            result.Data.ShouldNotBeNull();
            result.Data.Monthly.ShouldNotBeNull();
            result.Data.Quarterly.ShouldNotBeNull();
            result.Data.Yearly.ShouldNotBeNull();
            result.Data.Last5Years.ShouldNotBeNull();
            result.Data.AllTime.ShouldNotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_No_Projections_Found()
        {
            // Arrange
            var emptyProjections = DashboardMock.EmptyProjectProjectionList();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyProjections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("No project data available.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Projections_Is_Null()
        {
            // Arrange
            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<ProjectProjection>)null);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("No project data available.");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task Handle_Should_Build_Monthly_Chart_With_Four_Weeks()
        {
            // Arrange
            var projections = DashboardMock.GetCurrentMonthProjections(2024, 10);

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Monthly.ShouldNotBeNull();
            result.Data.Monthly.Count.ShouldBe(4); // 4 weeks
            result.Data.Monthly[0].Period.ShouldBe("Week 1");
            result.Data.Monthly[1].Period.ShouldBe("Week 2");
            result.Data.Monthly[2].Period.ShouldBe("Week 3");
            result.Data.Monthly[3].Period.ShouldBe("Week 4");
        }

        [Fact]
        public async Task Handle_Should_Calculate_Correct_Weekly_Project_Counts()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var projections = DashboardMock.GetCurrentMonthProjections(year, month);

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Monthly[0].Projects.ShouldBe(2); // Week 1 (Oct 1-7): 2 projects
            result.Data.Monthly[1].Projects.ShouldBe(1); // Week 2 (Oct 8-14): 1 project
            result.Data.Monthly[2].Projects.ShouldBe(2); // Week 3 (Oct 15-21): 2 projects
            result.Data.Monthly[3].Projects.ShouldBe(1); // Week 4 (Oct 22-31): 1 project
        }

        [Fact]
        public async Task Handle_Should_Build_Quarterly_Chart_With_Four_Quarters()
        {
            // Arrange
            var year = 2024;
            var projections = DashboardMock.GetQuarterlyProjections(year);

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Quarterly.ShouldNotBeNull();
            result.Data.Quarterly.Count.ShouldBe(4);
            result.Data.Quarterly[0].Period.ShouldBe($"Q1 {year}");
            result.Data.Quarterly[1].Period.ShouldBe($"Q2 {year}");
            result.Data.Quarterly[2].Period.ShouldBe($"Q3 {year}");
            result.Data.Quarterly[3].Period.ShouldBe($"Q4 {year}");
        }

        [Fact]
        public async Task Handle_Should_Calculate_Correct_Quarterly_Project_Counts()
        {
            // Arrange
            var year = 2024;
            var projections = DashboardMock.GetQuarterlyProjections(year);

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Quarterly[0].Projects.ShouldBe(2); // Q1: Jan 15, Feb 20
            result.Data.Quarterly[1].Projects.ShouldBe(1); // Q2: Apr 10
            result.Data.Quarterly[2].Projects.ShouldBe(2); // Q3: Jul 5, Jul 25
            result.Data.Quarterly[3].Projects.ShouldBe(1); // Q4: Oct 12
        }

        [Fact]
        public async Task Handle_Should_Build_Yearly_Chart_With_Twelve_Months()
        {
            // Arrange
            var projections = DashboardMock.GetProjectProjectionList();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Yearly.ShouldNotBeNull();
            result.Data.Yearly.Count.ShouldBe(12);
            result.Data.Yearly[0].Period.ShouldBe("Jan");
            result.Data.Yearly[1].Period.ShouldBe("Feb");
            result.Data.Yearly[11].Period.ShouldBe("Dec");
        }

        [Fact]
        public async Task Handle_Should_Calculate_Correct_Monthly_Counts_For_Current_Year()
        {
            // Arrange
            var projections = DashboardMock.GetProjectProjectionList();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Yearly[0].Projects.ShouldBe(2); // January: 2 projects (5th and 12th)
            result.Data.Yearly[1].Projects.ShouldBe(1); // February: 1 project (8th)
            result.Data.Yearly[2].Projects.ShouldBe(1); // March: 1 project (15th)
            result.Data.Yearly[5].Projects.ShouldBe(1); // June: 1 project (20th)
            result.Data.Yearly[8].Projects.ShouldBe(1); // September: 1 project (10th)
        }

        [Fact]
        public async Task Handle_Should_Build_Last5Years_Chart()
        {
            // Arrange
            var projections = DashboardMock.GetMultiYearProjections();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Last5Years.ShouldNotBeNull();
            result.Data.Last5Years.Count.ShouldBe(5);

            // Check if the years are consecutive and ordered
            for (int i = 0; i < result.Data.Last5Years.Count - 1; i++)
            {
                var currentYear = int.Parse(result.Data.Last5Years[i].Period);
                var nextYear = int.Parse(result.Data.Last5Years[i + 1].Period);
                (nextYear - currentYear).ShouldBe(1);
            }
        }

        [Fact]
        public async Task Handle_Should_Calculate_Correct_AllTime_Project_Counts()
        {
            // Arrange
            var projections = DashboardMock.GetMultiYearProjections();

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.AllTime.Count.ShouldBe(5); // 2020-2024
            result.Data.AllTime.First().Period.ShouldBe("2020");
            result.Data.AllTime.Last().Period.ShouldBe("2024");

            var year2021 = result.Data.AllTime.First(x => x.Period == "2021");
            year2021.Projects.ShouldBe(2); // Jun 20 and Sep 10, 2021

            var year2023 = result.Data.AllTime.First(x => x.Period == "2023");
            year2023.Projects.ShouldBe(2); // Apr 12 and Nov 8, 2023
        }

        [Fact]
        public async Task Handle_Should_Filter_Out_Default_CreatedAt_Dates()
        {
            // Arrange
            var projections = new List<ProjectProjection>
            {
                new ProjectProjection
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = default // Should be filtered out
                },
                new ProjectProjection
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = new DateTime(2024, 5, 15, 10, 0, 0, DateTimeKind.Utc)
                }
            };

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);

            // Only the valid date should be counted
            var totalProjects = result.Data.AllTime.Sum(c => c.Projects);
            totalProjects.ShouldBe(1);
        }

        [Fact]
        public async Task Handle_Should_Return_Zero_Counts_For_Months_With_No_Projects()
        {
            // Arrange
            var projections = new List<ProjectProjection>
            {
                new ProjectProjection
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
                }
            };

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.Yearly[0].Projects.ShouldBe(1); // January has 1
            result.Data.Yearly[1].Projects.ShouldBe(0); // February has 0
            result.Data.Yearly[2].Projects.ShouldBe(0); // March has 0
        }

        [Fact]
        public async Task Handle_Should_Handle_Projects_Created_On_Month_Boundaries()
        {
            // Arrange
            var projections = new List<ProjectProjection>
            {
                new ProjectProjection
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) // First day
                },
                new ProjectProjection
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = new DateTime(2024, 1, 31, 23, 59, 59, DateTimeKind.Utc) // Last day
                }
            };

            _projectReadRepositoryMock
                .Setup(repo => repo.GetAllProjectProjectionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(projections);

            var query = new GetProjectActivityQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.Yearly[0].Projects.ShouldBe(2); // Both should be counted in January
        }
    }
}
