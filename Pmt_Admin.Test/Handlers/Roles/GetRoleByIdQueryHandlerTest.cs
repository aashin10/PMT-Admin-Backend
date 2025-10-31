using AutoMapper;
using MediatR;
using Moq;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities; // Correct namespace for domain entities
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Roles
{
    public class GetRoleByIdQueryHandlerTest
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetRoleByIdQueryHandler _handler;

        public GetRoleByIdQueryHandlerTest()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetRoleByIdQueryHandler(_roleRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_RoleDto_When_Role_Exists()
        {
            // Arrange
            var roleId = 1;
            var role = new Role
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                Metadata = "{\"level\": \"high\"}",
                RolePermissions = new List<RolePermission>(),
                ProjectMembers = new List<ProjectMember>()
            };

            var expectedDto = new RoleDto
            {
                Id = roleId,
                Name = "Admin",
                Description = "Administrator role",
                Metadata = "{\"level\": \"high\"}"
            };

            _roleRepositoryMock
                .Setup(repo => repo.GetById(roleId))
                .Returns(Task.FromResult(role));

            _mapperMock
                .Setup(m => m.Map<RoleDto>(role))
                .Returns(expectedDto);

            var query = new GetRoleByIdQuery { Id = roleId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Name, result.Name);
            Assert.Equal(expectedDto.Description, result.Description);

            _roleRepositoryMock.Verify(repo => repo.GetById(roleId), Times.Once);
            _mapperMock.Verify(m => m.Map<RoleDto>(role), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Role_Not_Found()
        {
            // Arrange
            var roleId = 999;
            _roleRepositoryMock
                .Setup(repo => repo.GetById(roleId))
                .Returns(Task.FromResult<Role>(null));

            var query = new GetRoleByIdQuery { Id = roleId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _roleRepositoryMock.Verify(repo => repo.GetById(roleId), Times.Once);
            _mapperMock.Verify(m => m.Map<RoleDto>(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Call_Repository_Once()
        {
            // Arrange
            var roleId = 1;
            var role = new Role { Id = roleId, Name = "Tester" };
            var dto = new RoleDto { Id = roleId, Name = "Tester" };

            _roleRepositoryMock
                .Setup(repo => repo.GetById(roleId))
                .Returns(Task.FromResult(role));
            _mapperMock
                .Setup(m => m.Map<RoleDto>(role))
                .Returns(dto);

            var query = new GetRoleByIdQuery { Id = roleId };

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _roleRepositoryMock.Verify(repo => repo.GetById(roleId), Times.Once);
            _mapperMock.Verify(m => m.Map<RoleDto>(role), Times.Once);
        }
    }
}

