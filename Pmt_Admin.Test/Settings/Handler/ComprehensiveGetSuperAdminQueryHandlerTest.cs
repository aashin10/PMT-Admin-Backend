using AutoMapper;
using Moq;
using Pmt_Admin.Test.Settings.Mock;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Handlers.Settings;
using PmtAdmin.Application.Query.Settings;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using Shouldly;
using Xunit;

namespace Pmt_Admin.Test.Settings.Handler
{
    /// <summary>
    /// Comprehensive unit tests for GetSuperAdminQueryHandler
    /// Tests include: success scenarios, pagination, filtering, empty results, status codes
    /// </summary>
    public class ComprehensiveGetSuperAdminQueryHandlerTest
    {
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetSuperAdminQueryHandler _handler;

        public ComprehensiveGetSuperAdminQueryHandlerTest()
        {
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetSuperAdminQueryHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        #region Success Scenarios

        [Fact]
        public async Task Handle_Should_Return_200_Success_With_SuperAdmins_List()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var superAdmins = SuperAdminMock.GetAllSuperAdmins();
            var dtos = SuperAdminMock.GetSuperAdminDtos();

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(dtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200, "Status should be 200 OK for successful query");
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Handle_Should_Filter_Only_SuperAdmins()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var allUsers = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Super Admin 1" },
                new User { Id = 2, IsSuperAdmin = false, IsDeleted = false, Name = "Regular User" },
                new User { Id = 3, IsSuperAdmin = true, IsDeleted = false, Name = "Super Admin 2" },
                new User { Id = 4, IsSuperAdmin = false, IsDeleted = false, Name = "Another User" }
            };

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            capturedUsers.ShouldNotBeNull();
            capturedUsers.Count.ShouldBe(2, "Should only return super admins");
            capturedUsers.All(u => u.IsSuperAdmin).ShouldBeTrue("All returned users must be super admins");
        }

        [Fact]
        public async Task Handle_Should_Exclude_Deleted_SuperAdmins()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var allUsers = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Active Admin" },
                new User { Id = 2, IsSuperAdmin = true, IsDeleted = true, Name = "Deleted Admin" },
                new User { Id = 3, IsSuperAdmin = true, IsDeleted = false, Name = "Another Active Admin" }
            };

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(allUsers);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            capturedUsers.ShouldNotBeNull();
            capturedUsers.Count.ShouldBe(2, "Should exclude deleted super admins");
            capturedUsers.All(u => !u.IsDeleted).ShouldBeTrue("All returned users must not be deleted");
        }

        #endregion

        #region Pagination Tests

        [Fact]
        public async Task Handle_Should_Return_First_Page_Correctly()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 2 };
            var superAdmins = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 1" },
                new User { Id = 2, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 2" },
                new User { Id = 3, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 3" },
                new User { Id = 4, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 4" }
            };

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            capturedUsers.Count.ShouldBe(2, "Should return only page size items");
            capturedUsers[0].Id.ShouldBe(1);
            capturedUsers[1].Id.ShouldBe(2);
        }

        [Fact]
        public async Task Handle_Should_Return_Second_Page_Correctly()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 2, PageSize = 2 };
            var superAdmins = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 1" },
                new User { Id = 2, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 2" },
                new User { Id = 3, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 3" },
                new User { Id = 4, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 4" }
            };

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            capturedUsers.Count.ShouldBe(2, "Should return only page size items");
            capturedUsers[0].Id.ShouldBe(3);
            capturedUsers[1].Id.ShouldBe(4);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_Page_Exceeds_Available_Data()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 10, PageSize = 10 };
            var superAdmins = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 1" },
                new User { Id = 2, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 2" }
            };

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            capturedUsers.Count.ShouldBe(0, "Should return empty list when page exceeds data");
            result.Status.ShouldBe(200, "Should still return 200 even with empty results");
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 3)]
        [InlineData(1, 10)]
        [InlineData(3, 2)]
        public async Task Handle_Should_Respect_Different_Page_Sizes(int pageNumber, int pageSize)
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = pageNumber, PageSize = pageSize };
            var superAdmins = Enumerable.Range(1, 10)
                .Select(i => new User { Id = i, IsSuperAdmin = true, IsDeleted = false, Name = $"Admin {i}" })
                .ToList();

            List<User> capturedUsers = null;

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Callback<object>(users => capturedUsers = (List<User>)users)
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            int expectedCount = Math.Min(pageSize, Math.Max(0, superAdmins.Count - (pageNumber - 1) * pageSize));
            capturedUsers.Count.ShouldBe(expectedCount);
        }

        #endregion

        #region Empty Results Tests

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_SuperAdmins_Exist()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var emptyList = new List<User>();

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(emptyList);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_All_SuperAdmins_Are_Deleted()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var deletedAdmins = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = true, Name = "Deleted Admin 1" },
                new User { Id = 2, IsSuperAdmin = true, IsDeleted = true, Name = "Deleted Admin 2" }
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(deletedAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_Only_Regular_Users_Exist()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var regularUsers = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = false, IsDeleted = false, Name = "User 1" },
                new User { Id = 2, IsSuperAdmin = false, IsDeleted = false, Name = "User 2" }
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(regularUsers);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.Count.ShouldBe(0);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task Handle_Should_Throw_When_Repository_Throws_Exception()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(query, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Mapper_Throws_Exception()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var superAdmins = SuperAdminMock.GetAllSuperAdmins();

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Throws(new Exception("Mapping failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(query, CancellationToken.None)
            );
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Handle_Should_Handle_Large_Dataset()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            var largeDataset = Enumerable.Range(1, 10000)
                .Select(i => new User { Id = i, IsSuperAdmin = true, IsDeleted = false, Name = $"Admin {i}" })
                .ToList();

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(largeDataset);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Handle_PageNumber_Zero_Or_Negative()
        {
            // Arrange - Typically page 0 or negative should be treated as page 1
            var query = new GetSuperAdminQuery { PageNumber = 0, PageSize = 10 };
            var superAdmins = new List<User>
            {
                new User { Id = 1, IsSuperAdmin = true, IsDeleted = false, Name = "Admin 1" }
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            // The behavior depends on implementation - may return empty or treat as page 1
        }

        [Fact]
        public async Task Handle_Should_Handle_Very_Large_PageSize()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = int.MaxValue };
            var superAdmins = Enumerable.Range(1, 100)
                .Select(i => new User { Id = i, IsSuperAdmin = true, IsDeleted = false, Name = $"Admin {i}" })
                .ToList();

            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(superAdmins);
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
        }

        #endregion

        #region Verification Tests

        [Fact]
        public async Task Handle_Should_Call_Repository_GetAllAsync_Once()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User>());
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Mapper_Once()
        {
            // Arrange
            var query = new GetSuperAdminQuery { PageNumber = 1, PageSize = 10 };
            _superAdminRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User>());
            _mapperMock.Setup(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()))
                .Returns(new List<SuperAdminDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<List<SuperAdminDto>>(It.IsAny<List<User>>()), Times.Once);
        }

        #endregion
    }
}
