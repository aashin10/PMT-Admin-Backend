using MediatR;
using PmtAdmin.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Permissions
{
    public class GetAllPermissionsQuery : IRequest<List<PermissionDto>>
    {

    }
}
