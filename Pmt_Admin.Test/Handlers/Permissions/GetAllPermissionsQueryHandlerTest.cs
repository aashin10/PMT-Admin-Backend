using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Permissions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using Xunit;
    using PmtAdmin.Application.Handlers.Permissions;
    using PmtAdmin.Application.Query.Permissions;
    using PmtAdmin.Domain.Persistance;
    using PmtAdmin.Domain.Entities;
    using PmtAdmin.Application.Dto;

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
        public async Task Handle_ReturnsMappedPermissions()
        {
            // Arrange
            var permissions = new List<Permission>
        {
            new Permission { Id = 1, Name = "Read", Description = "Read permission" },
            new Permission { Id = 2, Name = "Write", Description = "Write permission" }
        };
            var permissionDtos = new List<PermissionDto>
        {
            new PermissionDto { Id = 1, Name = "Read", Description = "Read permission" },
            new PermissionDto { Id = 2, Name = "Write", Description = "Write permission" }
        };

            _permissionRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(permissions);
            _mapperMock.Setup(m => m.Map<List<PermissionDto>>(permissions))
                .Returns(permissionDtos);

            // Act
            var result = await _handler.Handle(new GetAllPermissionsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(permissionDtos, result);
            _permissionRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<PermissionDto>>(permissions), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var permissions = new List<Permission>();
            var permissionDtos = new List<PermissionDto>();

            _permissionRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(permissions);
            _mapperMock.Setup(m => m.Map<List<PermissionDto>>(permissions))
                .Returns(permissionDtos);

            // Act
            var result = await _handler.Handle(new GetAllPermissionsQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result);
            _permissionRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<PermissionDto>>(permissions), Times.Once);
        }
    }
}
