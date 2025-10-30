using Moq;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Test.Mock;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit; // Add this at the top of the file
using Shouldly; // Add this using directive for Assert methods

namespace Pmt_Admin.Test.Roles_and_Permissions.Handler
{
    public class GetRolesQueryHandlerTest
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetRolesQueryHandler _handler;

        public GetRolesQueryHandlerTest()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new GetRolesQueryHandler(_roleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Roles_When_Data_Exists()
        {
            // Arrange
            var roles = RoleMock.GetRoleList();
            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var firstRole = result.First();
            Assert.Equal("Admin", firstRole.Name);
            Assert.Equal(2, firstRole.UserCount);
            Assert.Equal(2, firstRole.Permissions.Count);
            Assert.Equal("CreateUser", firstRole.Permissions.First().Name);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Roles()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(RoleMock.EmptyRoleList());

            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Roles_Are_Null()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<PmtAdmin.Domain.Entities.Role>)null!);

            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Paginate_Correctly()
        {
            // Arrange
            var roles = RoleMock.GetRoleList();
            var query = new GetRolesQuery { Page = 2, PageSize = 1 };

            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("Manager", result.First().Name);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }
}

