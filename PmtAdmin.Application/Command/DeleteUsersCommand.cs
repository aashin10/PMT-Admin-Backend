using MediatR;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command
{
    public class DeleteUsersCommand : IRequest<ApiResponse<string>>
    {
        public IEnumerable<int> Ids { get; set; } = new List<int>();
    }
}