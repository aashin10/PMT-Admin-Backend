using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query
{
    public class GetUserByIdQuery : IRequest<ApiResponse<UserDto>>
    {
        public int Id { get; set; }
    }
}
