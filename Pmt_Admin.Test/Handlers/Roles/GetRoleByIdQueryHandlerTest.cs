using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using Pmt_Admin.Test.Handlers.Roles.Mock;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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
        public async Task Handle_WhenRoleExists_ReturnsSuccessWithRole()
        {
            // Arrange
            var roles = RoleMock.GetRoles();
            var role = roles.First();
            var roleDto = new RoleDto { Id = role.Id, Name = role.Name };
            var query = new GetRoleByIdQuery { Id = role.Id };

            _roleRepositoryMock
                .Setup(x => x.GetById(role.Id))
                .ReturnsAsync(role);
            
            _mapperMock
                .Setup(x => x.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(roleDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(role.Id);
        }

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ReturnsNull()
        {
            // Arrange
            var roleId = 999;
            var query = new GetRoleByIdQuery { Id = roleId };

            _roleRepositoryMock
                .Setup(x => x.GetById(roleId))
                .ReturnsAsync((Role)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}

