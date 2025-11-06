using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Permissions;
using PmtAdmin.Application.Query.Permissions;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using Pmt_Admin.Test.Handlers.Permissions.Mock;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Permissions
{
    public class GetAllPermissionsQueryHandlerTest
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllPermissionsQueryHandler _handler;

        public GetAllPermissionsQueryHandlerTest()
        {
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllPermissionsQueryHandler(_permissionRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenPermissionsExist_ReturnsSuccessWithPermissions()
        {
            // Arrange
            var permissions = PermissionMock.GetPermissions();
            var permissionDtos = permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description }).ToList();
            var query = new GetAllPermissionsQuery();

            _permissionRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(permissions);
            
            _mapperMock
                .Setup(x => x.Map<List<PermissionDto>>(It.IsAny<List<Permission>>()))
                .Returns(permissionDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(permissions.Count);
        }

        [Fact]
        public async Task Handle_WhenNoPermissionsExist_ReturnsEmptyList()
        {
            // Arrange
            var permissions = new List<Permission>();
            var query = new GetAllPermissionsQuery();

            _permissionRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(permissions);
            
            _mapperMock
                .Setup(x => x.Map<List<PermissionDto>>(It.IsAny<List<Permission>>()))
                .Returns(new List<PermissionDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
