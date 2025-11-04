using AutoMapper;
using Moq;
using Pmt_Admin.Test.Settings.Mock;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Handlers.Settings;
using PmtAdmin.Application.Query.Settings;
using PmtAdmin.Domain.Persistance.Settings;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Settings.Handler
{
    public class GetSuperAdminQueryHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly GetSuperAdminQueryHandler _handler;

        public GetSuperAdminQueryHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _handler = new GetSuperAdminQueryHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_SuperAdmins_Exist()
        {
            // Arrange
            var allUsers = SuperAdminMock.GetAllUsersIncludingNonSuperAdmins();
            var superAdminDtos = SuperAdminMock.GetSuperAdminDtoList();

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns(superAdminDtos);

            var query = new GetSuperAdminQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            //result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(3); // Only super admins, excluding deleted ones

            // Verify first super admin
            result.Data[0].Name.ShouldBe("Admin One");
            result.Data[0].Email.ShouldBe("admin1@example.com");
            result.Data[0].IsActive.ShouldBeTrue();

            _superAdminRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Filter_Out_Non_SuperAdmins()
        {
            // Arrange
            var allUsers = SuperAdminMock.GetAllUsersIncludingNonSuperAdmins();

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns((List<PmtAdmin.Domain.Entities.User> users) =>
                    users.Select(u => new SuperAdminDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        IsActive = u.IsActive
                    }).ToList());

            var query = new GetSuperAdminQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(3); // Only the 3 super admins
            result.Data.ShouldAllBe(dto => dto.Id != 10); // Regular user ID should not be included
        }

        [Fact]
        public async Task Handle_Should_Filter_Out_Deleted_SuperAdmins()
        {
            // Arrange
            var allUsers = SuperAdminMock.GetSuperAdminList();
            allUsers.Add(SuperAdminMock.GetDeletedSuperAdmin());

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns((List<PmtAdmin.Domain.Entities.User> users) =>
                    users.Select(u => new SuperAdminDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email
                    }).ToList());

            var query = new GetSuperAdminQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(3); // Deleted admin should be excluded
            result.Data.ShouldAllBe(dto => dto.Id != 5); // Deleted admin ID
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_SuperAdmins_Exist()
        {
            // Arrange
            var emptyList = SuperAdminMock.EmptySuperAdminList();

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(emptyList);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns(new List<SuperAdminDto>());

            var query = new GetSuperAdminQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(0);

            _superAdminRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Apply_Pagination_Correctly()
        {
            // Arrange
            var allUsers = SuperAdminMock.GetSuperAdminList();

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns((List<PmtAdmin.Domain.Entities.User> users) =>
                    users.Select(u => new SuperAdminDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email
                    }).ToList());

            var query = new GetSuperAdminQuery
            {
                PageNumber = 1,
                PageSize = 2
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Data.Count.ShouldBe(2); // Only 2 items per page
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Page_When_PageNumber_Is_Two()
        {
            // Arrange
            var allUsers = SuperAdminMock.GetSuperAdminList();

            _superAdminRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);

            _mapperMock
                .Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.User>>()))
                .Returns((List<PmtAdmin.Domain.Entities.User> users) =>
                    users.Select(u => new SuperAdminDto
                    {
                        Id = u.Id,
                        Name = u.Name
                    }).ToList());

            var query = new GetSuperAdminQuery
            {
                PageNumber = 2,
                PageSize = 2
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Data.Count.ShouldBe(1); // Only 1 item on page 2
            result.Data[0].Id.ShouldBe(3); // Third admin
        }
    }
}
